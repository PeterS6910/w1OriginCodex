using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals.PlatformPC;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Sys.Microsoft;
using Contal.IwQuick.Threads;
using Contal.IwQuick.Net;
using Contal.IwQuick.Remoting;
using Contal.Cgp.RemotingCommon;
using Contal.IwQuick;
using Contal.IwQuick.UI;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Microsoft.Win32;
using System.Management;

namespace Contal.Cgp.Client.Common
{
    /// <summary>
    /// main class of the CgpClient application
    /// </summary>
    public abstract class ACgpClientBase<TCgpClient, TCgpPluginManager>
        : ASingleton<TCgpClient>, ICgpClientBase
        where TCgpClient : ACgpClientBase<TCgpClient, TCgpPluginManager>
        where TCgpPluginManager : CgpClientPluginManager, new()
    {
        protected ACgpClientBase(TCgpClient dummyParameter) : base(dummyParameter)
        {
            _clientID = Guid.NewGuid();
        }

        /// <summary>
        /// plugin manager to load and manage visible plugins
        /// </summary>
        protected readonly TCgpPluginManager _pluginManager = new TCgpPluginManager();

        public const string PLUGIN_KEYWORD = "plugin";
        public const string ALARM_INSTRUCTION_STRING_KEY = "GlobalAlarmInstruction";

        internal const string ALARM_TYPE_LOCALIZATION_PREFIX = "AlarmType_";

        private IwQuick.Localization.LocalizationHelper _localizationHelper;

        private readonly Guid _clientID;

        public Guid ClientID
        {
            get { return _clientID; }
        }

        private string _loggedAs;

        public event Action<string> LoggedAsChanged;

        public string LoggedAs
        {
            get { return _loggedAs; }

            set
            {
                _loggedAs = value;

                if (LoggedAsChanged != null)
                    LoggedAsChanged(value);
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                return
                    IsAuthenticated() &&
                    !string.IsNullOrEmpty(_loggedAs);
            }
        }

        public bool IsServerOn
        {
            get
            {
                try
                {
                    if (MainServerProvider == null)
                        return false;

                    if (MainServerProvider.IsSessionValid)
                    {
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }


        public TCgpPluginManager PluginManager
        {
            get { return _pluginManager; }
        }

        private static bool EnabledSystemLanguage
        {
            get
            {
                RegistryKey registryKey;

                if (RegistryHelper.TryParseKey(CgpClientGlobals.RegPathClientSettings, true, out registryKey))
                {
                    try
                    {
                        var result = (int)registryKey.GetValue(CgpClientGlobals.CGP_ENABLED_SYSTEM_LANGUAGE);
                        return result == 1;
                    }
                    catch
                    {
                        return false;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// localization for forms in Cgp.Client
        /// </summary>
        public IwQuick.Localization.LocalizationHelper LocalizationHelper
        {
            get
            {
                if (_localizationHelper == null)
                {
                    _localizationHelper =
                        new IwQuick.Localization.LocalizationHelper(GetType().Assembly, EnabledSystemLanguage);

                    _localizationHelper.SetLanguage(Language);
                }

                return _localizationHelper;
            }

            set
            {
                _localizationHelper = value;
            }
        }

        protected abstract string Language
        {
            get;
        }

        /// <summary>
        /// default remoting peer for the connection to the Cgp Server
        /// </summary>
        private TcpRemotingPeer _remotingPeer;

        private RemotingProxyKeeper<ICgpServerRemotingProvider> _mainServerProviderKeeper;

        public void ReportOrmProviderProblem(Exception error)
        {
            if (null == error || null == _mainServerProviderKeeper)
                return;

            _mainServerProviderKeeper.ReportProblem(error);
        }

        public void AllowProxyRegaining()
        {
            if (null == _mainServerProviderKeeper)
                return;

            _mainServerProviderKeeper.AllowProxyRegaining();
        }

        /// <summary>
        /// returns instance of the OrmRemotingProvider's transparrent proxy
        /// </summary>
        public ICgpServerRemotingProvider MainServerProvider { get; private set; }

        public abstract bool GetRequirePINCardLogin
        {
            get; 
        }

        public abstract string ComPortName
        {
            get;
        }

        /// <summary>
        /// runs the visible modules and forms
        /// </summary>
        protected abstract void RunInternal();

        /// <summary>
        /// triggerred actions by main form startup
        /// </summary>
        public void Init()
        {
            try
            {
                LoadPlugins();
            }
            catch (Exception error)
            {
                Dialog.Error(error);
            }

            try
            {
                InitRemoting();
            }
            catch (Exception error)
            {
                Dialog.Error(error);
            }

            try
            {
                InitCNMP();
            }
            catch (Exception error)
            {
                Dialog.Error(error);
            }
        }

        protected abstract string PluginPath { get; }

        protected abstract void OnPluginSetChanged(ICgpClientPlugin[] parameter);
        protected abstract void OnPluginLoaded(ICgpClientPlugin parameter);

        /// <summary>
        /// loads all visible plugins
        /// </summary>
        protected virtual void LoadPlugins()
        {
            _pluginManager.PluginLoaded += OnPluginLoaded;
            _pluginManager.PluginSetChanged += OnPluginSetChanged;

            _pluginManager.LoadPlugins(
                PluginPath,
                false,
                null,
                true,
                null,
                null,
                "*.plugin.dll");
        }

        /// <summary>
        /// main processing of the CgpClinent
        /// </summary>
        public void Run()
        {
            Process currentProcess = Process.GetCurrentProcess();

            Process otherProcess = Process.GetProcessesByName(currentProcess.ProcessName)
                .FirstOrDefault(
                    process =>
                        process.Id != currentProcess.Id 
                        && Equals(GetMainModuleFilepath(process.Id), currentProcess.MainModule.FileName) 
                        && process.SessionId == currentProcess.SessionId);

            if (otherProcess != null)
            {
                NotifyProcessAlreadyRunning(otherProcess);
                return;
            }

            RunInternal();

            if (MainServerProvider == null)
                return;

            try
            {
                MainServerProvider.ClientDisconnect(
                    FriendlyName,
                    _clientID);
            }
            catch
            {
            }
        }

        // This method also works when the process is launched from Visual Studio in Debugger (32bit)
        private string GetMainModuleFilepath(int processId)
        {
            /* Use the following query to get process.MainModule.FileName to prevent 
             * 'process.MainModule.FileName' threw an exception of type 'System.ComponentModel.Win32Exception'
             *  Message: "A 32 bit processes cannot access modules of a 64 bit process."
             *  NativeErrorCode: 299  
             *  https://stackoverflow.com/questions/9501771/how-to-avoid-a-win32-exception-when-accessing-process-mainmodule-filename-in-c          
             */

            string wmiQueryString = "SELECT ProcessId, ExecutablePath FROM Win32_Process WHERE ProcessId = " + processId;
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            {
                using (var results = searcher.Get())
                {
                    ManagementObject mo = results.Cast<ManagementObject>().FirstOrDefault();
                    if (mo != null)
                    {
                        return (string)mo["ExecutablePath"];
                    }
                }
            }
            return null;
        }

        protected abstract string FriendlyName
        {
            get;
        }

        protected abstract void NotifyProcessAlreadyRunning(Process otherProcess);

        /// <summary>
        /// initializes all remoting communication to server
        /// </summary>

        private void InitRemoting()
        {
            try
            {
                ReconfigureRemoting();
            }
            // exception can be raised here, if Client unconfigured
            catch (Exception)
            {
                OnInitRemotingFailed();
                return;
            }

            if (null == _mainServerProviderKeeper)
            {
                _mainServerProviderKeeper = new RemotingProxyKeeper<ICgpServerRemotingProvider>(_remotingPeer);

                _mainServerProviderKeeper.ProxyGained += OnMainServerProviderProxyGained;
                _mainServerProviderKeeper.ProxyLost += OnMainServerProviderProxyLost;

                _mainServerProviderKeeper.Start();
            }
        }

        protected abstract void OnInitRemotingFailed();

        public void ReconfigureRemoting()
        {
            var aes =
                new AESSettings(
                    QuickHashes.GetSHA256(
                        Encoding.UTF8.GetString(CgpServerGlobals.Singleton.REMOTING_KEY)),
                    CgpServerGlobals.Singleton.REMOTING_SALT);

            if (_remotingPeer == null)
            {
                _remotingPeer =
                    TcpRemotingPeer.GetClientSide(
                        ServerAddress,
                        Port,
                        0,
                        aes);

                _remotingPeer.DynamicPortsLowBound = 
                    CgpServerGlobals.DEFAULT_REMOTING_CLIENT_PORT;

                _remotingPeer.DynamicPortsUpperBound = 
                    CgpServerGlobals.DEFAULT_REMOTING_CLIENT_PORT_UPPER;
            }
            else
            {
                _remotingPeer.Reconfigure(
                    false,
                    ServerAddress,
                    Port, 
                    0, 
                    aes);
            }

        }

        protected abstract int Port
        {
            get;
        }

        protected abstract string ServerAddress
        {
            get;
        }

        private CNMPAgent _cnmpAgent;

        private void InitCNMP()
        {
            bool isFree = TcpUdpPort.IsFreeUdp(34000, true);

            try
            {
                _cnmpAgent =
                    new CNMPAgent(
                        CgpClientGlobals.CNMP_INSTANCE_PREFIX + FriendlyName,
                        CgpClientGlobals.CNMP_TYPE,
                        null,
                        null)
                    {
                        Timeout = 300,
                        RetryCount = 2
                    };

                _cnmpAgent.Start();
            }
            catch (Exception ex)
            {
                if (isFree)
                    throw;

                throw new Exception(ex.Message + ". Port 34000 is not free");
            }
        }

        public CNMPAgent CNMP
        {
            get { return _cnmpAgent; }
        }

        public void ServerStoped()
        {
            OnMainServerProviderProxyLost(null);
            Singleton.AllowProxyRegaining();
        }

        private readonly object _lockProxyGainedProxyLost = new object();

        void OnMainServerProviderProxyLost(Type parameter)
        {
            lock (_lockProxyGainedProxyLost)
            {
                MainServerProvider = null;

                LoggedAs = null;

                OnDisconnected();
            }
        }

        protected abstract void OnDisconnected();

        public void SetDelegateForGainedLostProxy(
            Action<ICgpServerRemotingProvider> setGainedProxyDelegate,
            Action<Type> setLostProxyDelegate)
        {
            if (_mainServerProviderKeeper == null)
                return;

            _mainServerProviderKeeper.ProxyGained += setGainedProxyDelegate;
            _mainServerProviderKeeper.ProxyLost += setLostProxyDelegate;
        }

        public void RemoveDelegateForGainedLostProxy(
            Action<ICgpServerRemotingProvider> removeGainedProxyDelegate,
            Action<Type> removeLostProxyDelegate)
        {
            if (_mainServerProviderKeeper == null)
                return;

            _mainServerProviderKeeper.ProxyGained -= removeGainedProxyDelegate;
            _mainServerProviderKeeper.ProxyLost -= removeLostProxyDelegate;
        }

        private bool _isCreatedPluginProxyKeeper;

        //this value must be smaller than delay on server side
        private const long ConnectedClientsDelay = 5000;

        private void OnMainServerProviderProxyGained(ICgpServerRemotingProvider parameter)
        {
            lock (_lockProxyGainedProxyLost)
            {
                if (_isCreatedPluginProxyKeeper)
                    _pluginManager.PreRegisterAttachCallbackHandlers();
                else
                {
                    _isCreatedPluginProxyKeeper = true;

                    _pluginManager.CreatePluginsProxyKeeper(
                        ref _remotingPeer,
                        OnPluginProxyGained,
                        OnPluginProxyLost);
                }

                MainServerProvider = parameter;

                TryPerformClientUpgrade();

                if (MainServerProvider.ClientConnect(
                    FriendlyName,
                    _clientID))
                {
                    TimerManager.Static.StartTimer(
                        ConnectedClientsDelay,
                        false,
                        SendInfoConnected);

                    MainServerProvider.AttachCallbackHandler(StatusChangedDailyPlainHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(StatusChangedTimeZoneHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(DeletedObjectHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(CUDObjectHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(ChangeAlarmsHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(ColorSettingsChangedHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(AutoCloseSettingsChangedHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(CardGenerationFinisheEventHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(SqlServerOnlineStateChangedHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(ImportedPersonCountChangedHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(ImportedCardCountChangedHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(DatabaseBackupSettingsChangedHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(DatabaseExpirationEventlogSettingsChangedHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(DhcpServerSettingsChangedHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(SecuritySettingsChangedHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(AlarmSettingsChangedHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(RemoteServicesSettingsChangedHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(SerialPortSettingsChangedHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(NtpSettingsChangedHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(EventlogsChangedHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(AdvancedAccessSettingsChangedHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(AdvancedSettingsChangedHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(WarningSetMaxEventsCountHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(DemoPerionHasExpiredHandler.Singleton);
                    MainServerProvider.AttachCallbackHandler(AlarmStateChangedHandler.Singleton);

                    OnConnected();

                    RequestAuthenticateLoginAsync();
                }
                else
                {
                    Dialog.Error(_localizationHelper.GetString("ErrorLoggedCount"));
                    MainServerProvider = null;
                }

                if (CgpClientProxyGained == null)
                    return;

                try
                {
                    CgpClientProxyGained();
                }
                catch
                { }
            }
        }

        protected abstract void TryPerformClientUpgrade();

        protected abstract void OnConnected();

        private void PreRegisterAttachCallbackHandlers()
        {
            try
            {
                MainServerProvider.AttachCallbackHandler(
                    StatusChangedDailyPlainHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    StatusChangedTimeZoneHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    DeletedObjectHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    CUDObjectHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    ChangeAlarmsHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    ColorSettingsChangedHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    AutoCloseSettingsChangedHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    CardGenerationFinisheEventHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    SqlServerOnlineStateChangedHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    ImportedPersonCountChangedHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    ImportedCardCountChangedHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    DatabaseBackupSettingsChangedHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    DatabaseExpirationEventlogSettingsChangedHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    DhcpServerSettingsChangedHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    SecuritySettingsChangedHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    AlarmSettingsChangedHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    RemoteServicesSettingsChangedHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    SerialPortSettingsChangedHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    NtpSettingsChangedHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    EventlogsChangedHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    AdvancedAccessSettingsChangedHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    AdvancedSettingsChangedHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    WarningSetMaxEventsCountHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    DemoPerionHasExpiredHandler.Singleton);

                MainServerProvider.AttachCallbackHandler(
                    AlarmStateChangedHandler.Singleton);

                _pluginManager.PreRegisterAttachCallbackHandlers();
            }
            catch
            {

            }
        }

        private bool SendInfoConnected(TimerCarrier timerCarrier)
        {
            if (MainServerProvider == null)
                return false;

            try
            {
                MainServerProvider.SendInfoClientConnected(_clientID);
            }
            catch
            {

            }

            return true;
        }

        public event DVoid2Void CgpClientProxyGained;

        public double MilisecondsRemaining()
        {
            if (MainServerProvider == null)
                return 0;

            try
            {
                TimeSpan timeElapsed =
                    MainServerProvider
                        .GetServerDateTime()
                        .Subtract(MainServerProvider.GetServerDemoStartTime());

                return
                    MainServerProvider.GetServerDemoMaxTime() -
                       timeElapsed.TotalMilliseconds;
            }
            catch
            {

            }
            return 0;
        }

        public Action<ICgpClientPlugin> PluginProxyLost;

        private void OnPluginProxyLost(ICgpClientPlugin plugin)
        {
            if (PluginProxyLost != null)
                PluginProxyLost(plugin);
        }

        public Action<ICgpClientPlugin> PluginProxyGained;

        private void OnPluginProxyGained(ICgpClientPlugin plugin)
        {
            if (PluginProxyGained != null)
                PluginProxyGained(plugin);
        }

        public bool IsMainServerProviderAvailable
        {
            get
            {
                return MainServerProvider != null;
            }
        }

        public bool IsAuthenticated()
        {
            try
            {
                return MainServerProvider != null && MainServerProvider.IsSessionValid;
            }
            catch
            {
                return false;
            }
        }

        public bool IsConnectionLost(bool showDialog)
        {
            try
            {
                if (MainServerProvider != null)
                {
                    if (MainServerProvider.IsSessionValid)
                        return false;

                    PreRegisterAttachCallbackHandlers();
                }
                else
                {
                    if (showDialog)
                        Dialog.Error(LocalizationHelper.GetString("ErrorConnection"));
                }
            }
            catch (Exception error)
            {
                if (showDialog)
                    Dialog.Error(LocalizationHelper.GetString("ErrorConnection"));

                ReportOrmProviderProblem(error);
                return true;
            }

            LoggedAs = null;

            RequestAuthenticateLoginAsync();

            return true;
        }

        protected abstract void RequestAuthenticateLoginAsync();

        public void ForceLogout()
        {
            if (!IsLoggedIn)
                return;

            LogoutInternal();
        }

        public void Logout()
        {
            if (IsLoggedIn)
            {
                if (!Dialog.Question(_localizationHelper.GetString("SureToLogout")))
                    return;

                if (!LogoutInternal())
                    return;
            }

            RequestAuthenticateLoginAsync();
        }

        private bool LogoutInternal()
        {
            try
            {
                if (!Unauthenticate())
                    return false;

                string userName = _loggedAs;

                if (!PrepareLogout())
                    return false;

                MainServerProvider.Eventlogs.InsertEventClientLogout(userName);

                LoggedAs = null;
                OnLoggedOut();

                return true;
            }
            catch
            {
                return false;
            }
        }

        protected abstract bool PrepareLogout();

        protected abstract void OnLoggedOut();

        public bool Unauthenticate()
        {
            if (MainServerProvider == null)
                return true;

            try
            {
                if (!MainServerProvider.ServerGenaralOptionsProvider.IsLockCLientApplication())
                {
                    MainServerProvider.Unauthenticate();
                    return true;
                }

                string loggedAs = string.Empty;

                RequestUnlockLogin(ref loggedAs);

                if (string.IsNullOrEmpty(loggedAs))
                    return false;

                Exception error;

                if (!MainServerProvider.Unauthenticate(out error))
                {
                    Dialog.Error(
                        LocalizationHelper.GetString(
                            error is AccessDeniedException
                                ? "ErrorLogoutAccessDenied"
                                : "ErrorLogoutFailed"));

                    return false;
                }
            }
            catch
            {
            }

            return true;
        }

        protected abstract void RequestUnlockLogin(ref string loggedAs);

        public Dictionary<string, List<AccessPresentation>> GetPluginsListAccess()
        {
            return
                _pluginManager != null
                    ? _pluginManager.GetPluginsListAccess()
                    : null;
        }

        public void SaveAfterInsertWithData(string pluginFriendlyName, object obj, object clonedObj)
        {
            var aCgpClientPlugin =
                _pluginManager.GetLoadedPlugin(pluginFriendlyName);

            if (aCgpClientPlugin != null)
                aCgpClientPlugin.SaveAfterInsertWithData(obj, clonedObj);
        }

        public UserOpenedWindow GetOpenedWindowSettings(string formName, Login login)
        {
            return
                login != null ?
                    login.UserOpenedWindows.FirstOrDefault(windowSettings => windowSettings.FormName == formName)
                    : null;
        }

        private bool? _isOnlineCr;
        private string _firmwareInformation = string.Empty;
        private string _typeCRInformation = string.Empty;

        public string CardReaderTypeInformation
        {
            get { return _typeCRInformation; }
            set { _typeCRInformation = value; }
        }
        public string CardReaderFirmwareInformation
        {
            get { return _firmwareInformation; }
            set { _firmwareInformation = value; }
        }

        public bool IsCrOnline
        {
            get { return _isOnlineCr != null && (bool)_isOnlineCr; }
        }

        public bool? LoginCrState
        {
            get { return _isOnlineCr; }
            set
            {
                _isOnlineCr = value;

                if (_isOnlineCr == null || LoginCrOnlineStateChanged == null)
                    return;

                try
                {
                    LoginCrOnlineStateChanged((bool)_isOnlineCr);
                }
                catch
                {

                }
            }
        }

        public event DBool2Void LoginCrOnlineStateChanged;
        public event DString2Void LoginCrCardSwiped;

        public void CrOnlineStateChanged(bool online)
        {
            LoginCrState = online;
        }

        string _lastCard = string.Empty;
        public string LoginCrLastCard
        {
            get { return _lastCard; }
        }

        public void CrCardSwiped(string card)
        {
            _lastCard = card;

            if (LoginCrCardSwiped == null)
                return;

            try
            {
                LoginCrCardSwiped(card);
            }
            catch
            {

            }
        }

        public abstract void ClientLoginWithCard(string fullCardNumber);
        public abstract void ClientInfoWrongPIN();
        public abstract void LoginCardSwiped(string fullCardNumber);

        public List<string> GetListLoadedPlugins()
        {
            if (_pluginManager == null)
                return new List<string>();

            return
                _pluginManager
                    .Select(plugin => plugin._plugin.FriendlyName)
                    .ToList();
        }

        public bool IsLoadedPlugin(string pluginFriendlyName)
        {
            List<string> loadPlug = GetListLoadedPlugins();

            return
                loadPlug != null &&
                loadPlug.Any(actPluginFriendlyName => actPluginFriendlyName == pluginFriendlyName);
        }

        public IList<AlarmType> GetAlarmTypesWithPlugin()
        {
            var resultAt = new List<AlarmType>();

            IEnumerable<AlarmType> clientAlarmType = GetCgpClientAlarmTypes();
            resultAt.AddRange(clientAlarmType);

            List<string> loadedplugin = GetListLoadedPlugins();

            if (loadedplugin == null)
                return resultAt;

            foreach (string plg in loadedplugin)
            {
                ICgpClientPlugin clientPlugin = _pluginManager.GetLoadedPlugin(plg);

                if (clientPlugin == null)
                    continue;

                IList<AlarmType> pluginAlarmType = clientPlugin.GetPluginAlarmTypes();

                if (pluginAlarmType != null)
                    resultAt.AddRange(pluginAlarmType);
            }

            return resultAt;
        }

        private static IEnumerable<AlarmType> GetCgpClientAlarmTypes()
        {
            IList<AlarmType> pluginAlarmTypes = new List<AlarmType>();
            pluginAlarmTypes.Add(AlarmType.GeneralAlarm);
            return pluginAlarmTypes;
        }

        private ControlNotificationSettings _clientNotificationSettings;
        private readonly object _lockCnS = new object();

        public ControlNotificationSettings ClientControlNotificationSettings
        {
            get
            {
                lock (_lockCnS)
                    if (_clientNotificationSettings == null)
                        _clientNotificationSettings =
                            new ControlNotificationSettings
                            {
                                HintPosition = HintPosition.RightTop
                            };

                return _clientNotificationSettings;
            }
        }

        public abstract ExtendedVersion Version { get; }
    }
}
