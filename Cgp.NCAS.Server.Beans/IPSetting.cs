using System;

using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    [LwSerialize(320)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class IPSetting
    {
        [LwSerializeAttribute()]
        private bool _isDHCP;
        [LwSerializeAttribute()]
        private string _ipAddress;
        [LwSerializeAttribute()]
        private string _subnetMask;
        [LwSerializeAttribute()]
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

            if (gateway == string.Empty)
                _gateway = "0.0.0.0";
            else
                _gateway = gateway;
        }

        public bool Compare(IPSetting ipSetting)
        {
            if (ipSetting == null)
                return false;

            return (ipSetting.IsDHCP == IsDHCP && ipSetting.IsDHCP == true) ||
                ipSetting.IsDHCP == IsDHCP && ipSetting.IPAddress == IPAddress &&
                ipSetting.SubnetMask == SubnetMask && ipSetting.Gateway == Gateway;
        }
    }

    public class IPSettingsChangedHandler : Contal.IwQuick.Remoting.ARemotingCallbackHandler
    {
        private static volatile IPSettingsChangedHandler _singleton = null;
        private static object _syncRoot = new object();

        private Action<Guid, IPSetting> _ipSettingsChanged;

        public static IPSettingsChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new IPSettingsChangedHandler();
                    }

                return _singleton;
            }
        }

        public IPSettingsChangedHandler()
            : base("IPSettingsChangedHandler")
        {
        }

        public void RegisterIPSettingsChanged(Action<Guid, IPSetting> ipSettingsChanged)
        {
            _ipSettingsChanged += ipSettingsChanged;
        }

        public void UnregisterIPSettingsChanged(Action<Guid, IPSetting> ipSettingsChanged)
        {
            _ipSettingsChanged -= ipSettingsChanged;
        }

        public void RunEvent(Guid id, IPSetting ipSetting)
        {
            if (_ipSettingsChanged != null)
                _ipSettingsChanged(id, ipSetting);
        }
    }
}
