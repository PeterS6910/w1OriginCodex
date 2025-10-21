using System;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU
{
    [LwSerialize(320)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class IPSetting
    {
        [LwSerialize]
        private bool _isDHCP;
        [LwSerialize]
        private string _ipAddress;
        [LwSerialize]
        private string _subnetMask;
        [LwSerialize]
        private string _gateway;

        public bool IsDHCP { get { return _isDHCP; } }
        public string IPAddress { get { return _ipAddress; } }
        public string SubnetMask { get { return _subnetMask; } }
        public string Gateway { get { return _gateway; } }

        public IPSetting()
        {
        }

        public IPSetting(bool isDHCP, string ipAddress, string subnetMask, string gateway)
        {
            _isDHCP = isDHCP;
            _ipAddress = ipAddress;
            _subnetMask = subnetMask;
            _gateway = gateway;
        }
    }

    public static class IPSettings
    {
        public static IPSetting GetIPSettings()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "IPSetting IPSettings.GetIPSettings()");
            try
            {
                bool isDHCP;
                string ipAddress;
                string subnetMask;
                string gateway;
                CcuCore.Singleton.CeNetworkManagement.GetIPSettings(out isDHCP, out ipAddress, out subnetMask, out gateway);
                IPSetting result = new IPSetting(isDHCP, ipAddress, subnetMask, gateway);
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("IPSetting IPSettings.GetIPSettings return {0}", Log.GetStringFromParameters(result)));
                return result;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL,"IPSetting IPSettings.GetIPSettings return null");
                return null;
            }
        }

        public static bool SetIPSettings(bool isDHCP, string ipAddress, string subnetMask, string gateway)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("bool IPSettings.SetIPSettings(bool isDHCP, string ipAddress, string subnetMask, string gateway): [{0}]",
                Log.GetStringFromParameters(isDHCP, ipAddress, subnetMask, gateway)));
            try
            {
                if (isDHCP)
                {
                    CcuCore.Singleton.CeNetworkManagement.SetDynamic();
                }
                else
                {
                    CcuCore.Singleton.CeNetworkManagement.SetStatic(ipAddress, subnetMask, gateway);
                }

                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool IPSettings.SetIPSettings return true");
                return true;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL,"bool IPSettings.SetIPSettings return false");
                return false;
            }
        }
    }
}
