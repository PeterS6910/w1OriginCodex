using System;

using Contal.IwQuick;
using Contal.IwQuick.Net;
using Contal.IwQuick.Sys.Microsoft;
using Microsoft.Win32;
using Contal.Cgp.Globals;
using Contal.IwQuick.Sys;


namespace Contal.Cgp.Client
{
    /// <summary>
    /// class for CgpClient's general options serialization into the registry
    /// </summary>
    [Serializable]
    public sealed class GeneralOptions : ASingleton<GeneralOptions>
    {
        private string _serverAddress = string.Empty;
        private int _port = CgpServerGlobals.DEFAULT_REMOTING_SERVER_PORT;
        private string _friendlyName = string.Empty;
        private string _language = string.Empty;
        private string _comPortName = string.Empty;
        private bool _autoUpgradeClient = true;
        private bool _eventlogListReporter;
        private bool _alarmSoundNotification = true;
        private AlarmTypeBuzzer _alarmSoundInvocationType = AlarmTypeBuzzer.AlarmNotAcknowledged;
        private string _alarmSoundFile = @"Windows Exclamation.wav";
        private int _alarmSoundRepeatFrequency = 6;

        private bool _remotingSettingsChanged;
        private static readonly object _sync = new Object();

        public bool RemotingSettingsChanged
        {
            get
            {
                lock (_sync)
                {
                    bool ret = _remotingSettingsChanged;

                    return ret;
                }
            }
        }

        public void RemotingSettingsChangedReset()
        {
            _remotingSettingsChanged = false;
        }

        #region Properties
        public string ServerAddress
        {
            get { return _serverAddress; }
            set {
                Validator.CheckNullString(value);

                if (value != _serverAddress)
                {
                    _remotingSettingsChanged = true;
                    _serverAddress = value;
                }
            }
        }
        public int Port
        {
            get { return _port; }
            set {
                TcpUdpPort.CheckValidity(value);

                if (_port != value)
                {
                    _remotingSettingsChanged = true;
                    _port = value;
                }
            }
        }
        public string FriendlyName
        {
            get { return _friendlyName; }
            set { _friendlyName = value; }
        }
        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        public string ComPortName
        {
            get { return _comPortName; }
            set { _comPortName = value; }
        }

        public bool AutoUpgradeClient
        {
            get { return _autoUpgradeClient; }
            set { _autoUpgradeClient = value; }
        }

        public bool EventlogListReporter
        {
            get { return _eventlogListReporter; }
            set { _eventlogListReporter = value; }
        }

        public bool AlarmSoundNotification
        {
            get { return _alarmSoundNotification; }
            set { _alarmSoundNotification = value; }
        }

        public AlarmTypeBuzzer AlarmSoundInvocationType
        {
            get { return _alarmSoundInvocationType; }
            set { _alarmSoundInvocationType = value; }
        }

        public string AlarmSoundFile
        {
            get { return _alarmSoundFile; }
            set { _alarmSoundFile = value; }
        }

        public int AlarmSoundRepeatFrequency
        {
            get { return _alarmSoundRepeatFrequency; }
            set { _alarmSoundRepeatFrequency = value; }
        }
        #endregion

        private GeneralOptions() : base(null)
        {
            LoadFromRegistry();
        }

        public bool IsConfigured()
        {
            bool ic= true;
            try
            {
                Validator.CheckNull(_serverAddress);
                Validator.CheckZero(_port);
                Validator.CheckNegativeInt(_port);
            }
            catch
            {
                ic = false;
            }

            return ic;
        }

        public void LoadFromRegistry()
        {
            try
            {
                RegistryKey registryKey;

                if (RegistryHelper.TryParseKey(CgpClientGlobals.RegPathClientRoot, true, out registryKey))
                {
                    var defaultValue = (string)registryKey.GetValue(null);

                    if (string.IsNullOrEmpty(defaultValue))
                    {
                        RegistryHelper.GetOrAddKey(
                            CgpClientGlobals.RegPathClientRoot).
                            SetValue(null, AppDomain.CurrentDomain.BaseDirectory, RegistryValueKind.String);
                    }
                }

                if (RegistryHelper.TryParseKey(CgpClientGlobals.RegPathRemotingSettings, true, out registryKey))
                {
                    try
                    {
                        _friendlyName = (string)registryKey.GetValue(CgpClientGlobals.CGP_CLIENT_FRIENDLY_NAME);
                    }
                    catch
                    {
                        _friendlyName = string.Empty;
                    }
                    try
                    {
                        _serverAddress = (string)registryKey.GetValue(CgpClientGlobals.CGP_SERVER_HOST_IP);
                    }
                    catch
                    {
                        _serverAddress = string.Empty;
                    }

                    try
                    {
                        _port = (int)registryKey.GetValue(CgpClientGlobals.CGP_SERVER_HOST_PORT);
                        if (_port == 0)
                            _port = CgpServerGlobals.DEFAULT_REMOTING_SERVER_PORT;
                    }
                    catch
                    {
                        _port = CgpServerGlobals.DEFAULT_REMOTING_SERVER_PORT;
                    }
                }
                else
                {
                    _serverAddress = string.Empty;
                    _port = 0;
                    _friendlyName = string.Empty;
                }

                if (RegistryHelper.TryParseKey(CgpClientGlobals.RegPathClientSettings, true, out registryKey))
                {
                    try
                    {
                        _language = (string)registryKey.GetValue(CgpClientGlobals.CGP_DEFAULT_LANGUAGE);

                        if (string.IsNullOrEmpty(_language))
                            _language = CgpClientGlobals.DEFAULT_LANGUAGE_SELECT;
                    }
                    catch
                    {
                        _language = CgpClientGlobals.DEFAULT_LANGUAGE_SELECT;
                    }

                    try
                    {
                        _autoUpgradeClient = bool.Parse(registryKey.GetValue(CgpClientGlobals.CGP_AUTO_UPGRADE_CLIENT).ToString());
                    }
                    catch
                    {
                        _autoUpgradeClient = true;
                    }

                    try
                    {
                        _eventlogListReporter = bool.Parse(registryKey.GetValue(CgpClientGlobals.CGP_EVENTLOG_LIST_REPORTER).ToString());
                    }
                    catch
                    {
                        _eventlogListReporter = false;
                    }

                    try
                    {
                        _alarmSoundNotification = bool.Parse(registryKey.GetValue(CgpClientGlobals.CGP_ALARM_SOUND_NOTIFICATION).ToString());
                    }
                    catch
                    {
                        _alarmSoundNotification = true;
                    }

                    try
                    {
                        _comPortName = (string)registryKey.GetValue(CgpClientGlobals.CGP_COM_PORT_NAME);
                    }
                    catch
                    {
                        _comPortName = string.Empty;
                    }

                    try
                    {
                        int alarmBT = (int)registryKey.GetValue(CgpClientGlobals.CGP_ALARM_SOUND_INVOCATION_TYPE);
                        _alarmSoundInvocationType = (AlarmTypeBuzzer)alarmBT;
                    }
                    catch
                    {
                        _alarmSoundInvocationType = AlarmTypeBuzzer.AlarmNotAcknowledged;
                    }

                    try
                    {
                        _alarmSoundFile = (string)registryKey.GetValue(CgpClientGlobals.CGP_ALARM_SOUND_FILE);
                        if (string.IsNullOrEmpty(_alarmSoundFile))
                            _alarmSoundFile = @"Windows Exclamation.wav";
                    }
                    catch
                    {
                        _alarmSoundFile = @"Windows Exclamation.wav";
                    }

                    try
                    {
                        _alarmSoundRepeatFrequency = (int)registryKey.GetValue(CgpClientGlobals.CGP_ALARM_SOUND_REPEAT_FREQUENCY);
                    }
                    catch
                    {
                        _alarmSoundRepeatFrequency = 6;
                    }
                }
                else
                {
                    _language = CgpClientGlobals.DEFAULT_LANGUAGE_SELECT;
                    _comPortName = string.Empty;
                }
            }
            catch(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public bool IsRemotingValid()
        {
            return (Validator.IsNotNullString(_serverAddress) &&
                TcpUdpPort.IsValid(Port, false));
        }

        public void SaveOtherSettingsToRegistry()
        {
            string registryKey = CgpClientGlobals.RegPathClientSettings;

            RegistryKey rk = RegistryHelper.GetOrAddKey(registryKey);

            if (null == rk)
                throw new DoesNotExistException(registryKey);

            rk.SetValue(CgpClientGlobals.CGP_AUTO_UPGRADE_CLIENT, _autoUpgradeClient, RegistryValueKind.String);
            rk.SetValue(CgpClientGlobals.CGP_EVENTLOG_LIST_REPORTER, _eventlogListReporter, RegistryValueKind.String);
            rk.SetValue(CgpClientGlobals.CGP_ALARM_SOUND_NOTIFICATION, _alarmSoundNotification, RegistryValueKind.String);
            rk.SetValue(CgpClientGlobals.CGP_ALARM_SOUND_INVOCATION_TYPE, _alarmSoundInvocationType, RegistryValueKind.DWord);
            rk.SetValue(CgpClientGlobals.CGP_ALARM_SOUND_FILE, _alarmSoundFile, RegistryValueKind.String);
            rk.SetValue(CgpClientGlobals.CGP_ALARM_SOUND_REPEAT_FREQUENCY, _alarmSoundRepeatFrequency, RegistryValueKind.DWord);
        }

        public void SaveClientSettingsToRegisty()
        {
            string registryKey = CgpClientGlobals.RegPathRemotingSettings;

            RegistryKey rk = RegistryHelper.GetOrAddKey(registryKey);

            if (null == rk)
                throw new DoesNotExistException(registryKey);
           
            rk.SetValue(CgpClientGlobals.CGP_SERVER_HOST_IP, _serverAddress, RegistryValueKind.String);
            rk.SetValue(CgpClientGlobals.CGP_SERVER_HOST_PORT, _port, RegistryValueKind.DWord);
            rk.SetValue(CgpClientGlobals.CGP_CLIENT_FRIENDLY_NAME, _friendlyName, RegistryValueKind.String);
        }

        public void SaveLanguageSettingsToRegistry()
        {
            string registryKey = CgpClientGlobals.RegPathClientSettings;

            RegistryKey rk = RegistryHelper.GetOrAddKey(registryKey);

            if (null == rk)
                throw new DoesNotExistException(registryKey);

            rk.SetValue(CgpClientGlobals.CGP_DEFAULT_LANGUAGE, _language, RegistryValueKind.String);
        }

        public void SaveComPortSettingsToRegistry()
        {
            string registryKey = CgpClientGlobals.RegPathClientSettings;

            RegistryKey rk = RegistryHelper.GetOrAddKey(registryKey);

            if (null == rk)
                throw new DoesNotExistException(registryKey);

            rk.SetValue(CgpClientGlobals.CGP_COM_PORT_NAME, _comPortName, RegistryValueKind.String);
        }
    }
}
