using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.ORM;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.IwQuick;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Net;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Sys.Microsoft;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using Contal.LwDhcp;
using Contal.IwQuick.Data;
using Contal.LwSerialization;
using Microsoft.Win32;

namespace Contal.Cgp.Server
{
    /// <summary>
    /// main core instance class for the CGP hosting application
    /// </summary>
    public class CgpServer : AServiceAndConsoleApp
    {
        /// <summary>
        /// internal CGP server versioning
        /// </summary>
        public ExtendedVersion Version { get; private set; }

        /// <summary>
        /// central manager of the included plugins
        /// </summary>

        public CgpServerPluginManager PluginManager { get; private set; }

        /// <summary>
        /// default remoting peer for server operations
        /// </summary>
        private TcpRemotingPeer _remotingPeer;

        public LocalizationHelper LocalizationHelper { get; private set; }

        /// <summary>
        /// instantiates all singleton dependendencies
        /// </summary>
        private CgpServer()
            : base(CgpServerGlobals.TERMINATION_WAIT_TIME * 1000)
        {
            ClientUpgradesPath = QuickPath.AssemblyStartupPath + @"\" + CLIENT_UPGRADES_DIRECTORY_NAME;
            IsFirst = true;
            DemoLicense = false;
            Version = new ExtendedVersion(
                typeof(CgpServer),
                true,
                null,
#if !DEBUG
#if RELEASE_SPECIAL
                DevelopmentStage.Testing
#else
                DevelopmentStage.Release
#endif
#else
 DevelopmentStage.Testing
#endif
);

            LocalizationHelper = new LocalizationHelper(GetType().Assembly);
            LocalizationHelper.SetLanguage("English");

            CreateAlarmsQueue();
            RemotingSessionHandler.Singleton.AddDelegateSessionTimeOut(OnSessionTimeOut);

            TypeCache.LoadAssemblies(
                GetType().Assembly,
                typeof(Alarm).Assembly,
                typeof(AOrmObject).Assembly);
        }

        public void SetServerAndPluginsRequiredLicenceProperties(Dictionary<string, object> properties)
        {
            try
            {
                _needRestart = false;
                var requiredProperties = GetRequiredLicencePropertyNamesAndTypes();

                var relevantProperties =
                    properties
                        .Where(
                            property =>
                                property.Value != null &&
                                requiredProperties.ContainsKey(property.Key) &&
                                requiredProperties[property.Key] ==
                                    (property.Value.GetType())).ToDictionary(obj => obj.Key, obj => obj.Value);

                if (_requiredLicenceProperties.Count > 0)
                {
                    if (_requiredLicenceProperties.Any(obj => !relevantProperties.ContainsKey(obj.Key)) ||
                        _requiredLicenceProperties.Any(
                            obj => obj.Value.GetType() != typeof(string)
                                && ((IComparable)obj.Value).CompareTo(relevantProperties[obj.Key]) > 0))
                    {
                        _needRestart = true;
                    }
                }

                _requiredLicenceProperties.Clear();

                foreach (var property in relevantProperties)
                    _requiredLicenceProperties[property.Key] = property.Value;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            foreach (ACgpServerPlugin plugin in PluginManager.GetLoadedPlugins())
            {
                if (plugin.SetRequiredLicenceProperties(properties))
                    _needRestart = true;
            }
        }

        private static volatile CgpServer _singleton;
        private static readonly object _syncRoot = new object();

        /// <summary>
        /// main CGP server singleton
        /// </summary>
        public static CgpServer Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                        if (_singleton == null)
                            _singleton = new CgpServer();

                return _singleton;
            }
        }

        public const string CLIENT_UPGRADES_DIRECTORY_NAME = "Client Upgrades";

        public string ClientUpgradesPath { get; set; }

        public bool IsFirst { get; private set; }

        public void SetIsNotFirst()
        {
            IsFirst = false;
        }

        /// <summary>
        /// default log for the CGP server
        /// </summary>
        public Log LogCgpServer { get; private set; }

        /// <summary>
        /// indicates, whether the current CGP server instance run in demo mode
        /// </summary>
        public bool DemoLicense { get; set; }

        public DateTime DemoStartTime { get; set; }

        private readonly Dictionary<string, object> _requiredLicenceProperties =
            new Dictionary<string, object>();

        private bool _needRestart;

        public string DHCPServerFile = @"\data.xml";

        private volatile DHCPServer _dhcpServer;
        /// <summary>
        /// DHCPServer instance
        /// </summary>
        public DHCPServer DhcpServer
        {
            get { return _dhcpServer ?? (_dhcpServer = new DHCPServer(67, 68)); }

            set { _dhcpServer = value; }
        }

        public bool NeedRestart
        {
            get { return _needRestart; }
        }

        public bool AutoStartDHCP { get; set; }

        public bool StartDHCP(bool toStart)
        {
            try
            {
                if (toStart)
                {
                    DhcpServer.Load(QuickPath.AssemblyStartupPath + Singleton.DHCPServerFile);
                    DhcpServer.Start();
                }
                else
                {
                    DhcpServer.Stop();
                    DhcpServer = null;
                }

                return true;
            }
            catch (Exception)
            {
            }

            return false;
        }

        private Dictionary<Guid, bool> _connectedClients =
            new Dictionary<Guid, bool>();

        public int ConnectedClients
        {
            get { return _connectedClients.Count; }
        }

        private const long RefreshConClientsDelay = 10000;
        private TimerManager _tmRefreshConnectedClients = new TimerManager();

        public bool AddConnectedClient(Guid clientId)
        {
            if (_connectedClients == null)
                _connectedClients = new Dictionary<Guid, bool>();

            if (_connectedClients.ContainsKey(clientId))
                return false;

            _connectedClients.Add(clientId, true);

            if (_tmRefreshConnectedClients == null)
                _tmRefreshConnectedClients = new TimerManager();

            if (_tmRefreshConnectedClients.Count == 0)
                _tmRefreshConnectedClients.StartTimer(
                    RefreshConClientsDelay,
                    false,
                    RefreshConnectedClients);

            return true;
        }

        public void RemoveFromConnectedClients(Guid clientId)
        {
            if (_connectedClients == null)
                return;

            if (_connectedClients.ContainsKey(clientId))
                _connectedClients.Remove(clientId);

            if (_connectedClients.Keys.Count == 0)
                _tmRefreshConnectedClients.StopAll();
        }

        public void StopServiceIfRunning()
        {
            if (!RunningAsService)
                return;

            try
            {
                var sc = new ServiceController(
                    CgpServerGlobals.CGP_SERVICE_NAME);

                sc.Stop();
                LogCgpServer.Error(
                    "\r\nService " + CgpServerGlobals.CGP_SERVICE_NAME +
                    " stopped\r\n");

            }
            catch
            {
            }
        }

        public Dictionary<string, Type> GetRequiredLicencePropertyNamesAndTypes()
        {
            var result = new Dictionary<string, Type>();

            foreach (
                RequiredLicenceProperties property in
                    Enum.GetValues(typeof(RequiredLicenceProperties)))
                switch (property)
                {
                    case RequiredLicenceProperties.Edition:
                        result.Add(property.ToString(), typeof(string));
                        break;

                    case RequiredLicenceProperties.ConnectionCount:
                        result.Add(property.ToString(), typeof(int));
                        break;

                    case RequiredLicenceProperties.CISIntegration:
                        result.Add(property.ToString(), typeof(bool));
                        break;

                    case RequiredLicenceProperties.OfflineImport:
                        result.Add(property.ToString(), typeof(bool));
                        break;

                    case RequiredLicenceProperties.MajorVersion:
                        result.Add(property.ToString(), typeof(int));
                        break;

                    case RequiredLicenceProperties.IdManagement:
                        result.Add(property.ToString(), typeof(bool));
                        break;

                    case RequiredLicenceProperties.MaxSubsiteCount:
                        result.Add(property.ToString(), typeof(int));
                        break;

                    default:
                        result.Add(property.ToString(), typeof(string));
                        break;
                }
            return result;
        }

        public Dictionary<string, Type> GetRequiredLicencePropertiesAllPlugins()
        {
            if (PluginManager == null)
                return new Dictionary<string, Type>();

            return
                PluginManager.GetLoadedPlugins()
                    .Select(plugin => plugin.GetRequiredLicenceProperties())
                    .Where(pluginProperties => pluginProperties != null)
                    .SelectMany(pluginProperties => pluginProperties)
                    .ToDictionary(
                        property => property.Key,
                        property => property.Value);
        }

        public Dictionary<string, Type> GetRequiredLicencePropertiesServerAndAllPlugins()
        {
            var allProperties = new Dictionary<string, Type>();

            var serverProperties = GetRequiredLicencePropertyNamesAndTypes();
            var pluginsProperties = GetRequiredLicencePropertiesAllPlugins();

            if (serverProperties != null)
                foreach (KeyValuePair<string, Type> property in serverProperties)
                    allProperties.Add(property.Key, property.Value);

            if (pluginsProperties == null)
                return allProperties;

            foreach (var property in pluginsProperties)
                allProperties.Add(property.Key, property.Value);

            return allProperties;
        }

        public bool GetLocalisedLicencePropertyName(string propertyName, out string localisedName)
        {
            return
                GetLocalisedLicencePropertyName(
                    propertyName,
                    GeneralOptions.Singleton.CurrentLanguage,
                    out localisedName);
        }

        public bool GetLocalisedLicencePropertyName(
            string propertyName,
            string language,
            out string localisedName)
        {
            localisedName = string.Empty;

            string result = LocalizationHelper.GetString(propertyName, language);

            if (string.IsNullOrEmpty(result) ||
                    result.StartsWith(LocalizationHelper.NO_TRANSLATION))
                return false;

            localisedName = result;
            return true;
        }

        public bool GetLocalisedLicencePropertyNameAllPlugins(
            string propertyName,
            out string localisedName)
        {
            return
                GetLocalisedLicencePropertyNameAllPlugins(
                    propertyName,
                    GeneralOptions.Singleton.CurrentLanguage,
                    out localisedName);
        }

        public bool GetLocalisedLicencePropertyNameAllPlugins(
            string propertyName,
            string language,
            out string localisedName)
        {
            localisedName = string.Empty;

            if (PluginManager == null)
                return false;

            foreach (ACgpServerPlugin plugin in PluginManager.GetLoadedPlugins())
                if (plugin.GetLocalisedLicencePropertyName(
                        propertyName,
                        language,
                        out localisedName))
                    return true;

            return false;
        }

        public bool GetLocalisedLicencePropertyNameServerAndAllPlugins(
            string propertyName,
            out string localisedName)
        {
            return
                GetLocalisedLicencePropertyNameServerAndAllPlugins(
                    propertyName,
                    GeneralOptions.Singleton.CurrentLanguage,
                    out localisedName);
        }

        public bool GetLocalisedLicencePropertyNameServerAndAllPlugins(
            string propertyName,
            string language,
            out string localisedName)
        {
            if (GetLocalisedLicencePropertyName(propertyName, language, out localisedName))
                return true;

            foreach (ACgpServerPlugin plugin in PluginManager.GetLoadedPlugins())
                if (plugin.GetLocalisedLicencePropertyName(
                        propertyName,
                        language,
                        out localisedName))
                    return true;

            return false;
        }

        private void TrySetLanguage(string language)
        {
            if (string.IsNullOrEmpty(language))
                return;

            try
            {
                LocalizationHelper.SetLanguage(language);
            }
            catch
            {
            }
        }

        private bool RefreshConnectedClients(TimerCarrier timerCarrier)
        {
            var inactiveClients = new LinkedList<Guid>();

            lock (_connectedClients)
                try
                {
                    foreach (
                        var client in
                            new LinkedList<Guid>(_connectedClients.Keys))
                    {
                        if (_connectedClients[client] == false)
                            inactiveClients.AddLast(client);

                        _connectedClients[client] = false;
                    }

                    foreach (Guid inactiveClient in inactiveClients)
                        _connectedClients.Remove(inactiveClient);

                    if (_connectedClients.Keys.Count == 0)
                        return false;
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }

            return true;
        }

        public void ClientIsConnected(Guid clientId)
        {
            if (_connectedClients == null)
                return;

            if (_connectedClients.ContainsKey(clientId))
                _connectedClients[clientId] = true;
        }

        private void SimpleAbout()
        {
            if (RunningAsService)
                return;

            Console.Title = CgpServerGlobals.NOVA_SERVER + " configuration";

            ConsoleColor fgc = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine();
            Console.WriteLine(StringConstants.TAB + CgpServerGlobals.NOVA_SERVER + " console");
            Console.WriteLine(StringConstants.TAB + Version);
            Console.WriteLine();
            //Console.WriteLine();

            Console.ForegroundColor = fgc;

            if (Version.DevelopmentStage == DevelopmentStage.Testing ||
                    Version.DevelopmentStage == DevelopmentStage.Beta ||
                    Version.DevelopmentStage == DevelopmentStage.Alpha)
                ExtendedConsole.ErrorWriteLine(
                    "------------------------------------------------\n!!! THIS VERSION IS FOR TESTING PURPOSE ONLY !!!\n------------------------------------------------\n");
        }

        private readonly ManualResetEvent _databaseVerificationEnded =
            new ManualResetEvent(false);

        private void LoadPlugins()
        {
            if (PluginManager != null)
                return;

            PluginManager = new CgpServerPluginManager();

            PluginManager.PluginLoaded += OnPluginLoaded;
            PluginManager.PluginFailed += OnPluginFailed;
            PluginManager.PluginUnlicensed += OnPluginUnlincensed;
            PluginManager.PluginSetLoaded += OnPluginSetLoaded;

            if (PluginManager.LoadPlugins(
                    QuickApp.StartupPath,
                    false,
                    AppDomain.CurrentDomain,
                    DemoLicense,
                    LicenseHelper.Singleton.GetPlugins(),
                    "*.plugin.dll") == 0)
                // to this directly at least for CGP server
                SafeThread.StartThread(VerifyDatabaseStructure);
        }

        private void LoadAndProcessLicenseProperties()
        {
            LicenseHelper.Singleton.SetServerAndPluginsRequiredLicenceProperties();
        }

        public bool IsAllowedMajorVersion()
        {
            string localisedName;
            object value;

            if (!GetLicencePropertyInfo(
                    RequiredLicenceProperties.MajorVersion.ToString(),
                    out localisedName, out value) ||
                !(value is int))
            {
                LogCgpServer.Error("Could not get major version property from license");
                return false;
            }

            var allowedMajorVersion = (int)value;

            if (Version.Major == allowedMajorVersion)
                return true;

            LogCgpServer.Error(
                string.Format(
                    "License does not support server major version: {0}. Supported version is: {1}",
                    Singleton.Version.Major,
                    allowedMajorVersion));

            return false;
        }

        private static LinkedList<Assembly> GetAssemblies(string filePattern)
        {
            return
                new LinkedList<Assembly>(
                    Directory
                        .GetFiles(
                            QuickApp.StartupPath,
                            filePattern,
                            SearchOption.TopDirectoryOnly)
                        .Select<string, Assembly>(GetAssemblyFromPath)
                        .Where(assembly => assembly != null));
        }

        private static Assembly GetAssemblyFromPath(string path)
        {
            try
            {
                return Assembly.ReflectionOnlyLoadFrom(path);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool TimetecCommunicationIsEnabled { get; set; }

        private void VerifyDatabaseStructure()
        {
            ConnectionString connectString =
                ConnectionString.LoadFromRegistry(
                    CgpServerGlobals.REGISTRY_CONNECTION_STRING);

            ConnectionString connectStringExternDatabase =
                ConnectionString.LoadFromRegistry(
                    CgpServerGlobals.REGISTRY_CONNECTION_STRING_EXTERN_DATABASE);

            var assemblies = GetAssemblies("*Beans.dll");
            var assembliesExternDatabase = GetAssemblies("*Beans.Extern.dll");

            var mng =
                new DBSCreator.Manager(
                    GetType().Assembly,
                    connectString,
                    assemblies,
                    connectStringExternDatabase,
                    assembliesExternDatabase,
                    RunningAsService,
                    LocalizationHelper.ActualLanguage);

            switch (GeneralMode)
            {
                case GeneralMode.Runtime:

                    bool connectionStringWasEdited;

                    if (!mng.Process(out connectionStringWasEdited))
                        break;

                    _verifyDatabaseStructureSucceeded = true;

                    if (connectionStringWasEdited)
                        try
                        {
                            connectString.SaveToRegistry(
                                CgpServerGlobals.REGISTRY_CONNECTION_STRING);

                            connectStringExternDatabase.SaveToRegistry(
                                CgpServerGlobals
                                    .REGISTRY_CONNECTION_STRING_EXTERN_DATABASE);
                        }
                        catch (DoesNotExistException error)
                        {
                            LogCgpServer.Error(
                                "Unable to store values into registry " +
                                error.Name);
                        }
                        catch (Exception)
                        {
                        }

                    break;

                case GeneralMode.Maintenance:

                    if (!mng.RunSettings())
                        break;

                    _verifyDatabaseStructureSucceeded = true;

                    try
                    {
                        connectString
                            .SaveToRegistry(
                                CgpServerGlobals.REGISTRY_CONNECTION_STRING);

                        connectStringExternDatabase
                            .SaveToRegistry(
                                CgpServerGlobals.REGISTRY_CONNECTION_STRING_EXTERN_DATABASE);
                    }
                    catch (DoesNotExistException error)
                    {
                        LogCgpServer.Error(
                            "Unable to store values into registry " +
                            error.Name);
                    }
                    catch (Exception)
                    {
                    }

                    break;

                case GeneralMode.Verification:
#if DEBUG
                    try
                    {

                        NhHelper.Singleton.ConnectionString = connectString;

                        NhHelper.Singleton.CreateSchema(assemblies);
                        //NhHelper.Singleton.UpdateSchema(assemblies);

                        ExtendedConsole.Info("Everything regarding NH is correct");

                        _verifyDatabaseStructureSucceeded = true;
                    }
                    catch (Exception error)
                    {
                        ExtendedConsole.Error(error.GetType().ToString());
                        ExtendedConsole.Error(error.Message);

                        Exception innerException = error.InnerException;

                        while (innerException != null)
                        {
                            Console.WriteLine();

                            ExtendedConsole.Error(innerException.GetType().ToString());
                            ExtendedConsole.Error(innerException.Message);

                            innerException = innerException.InnerException;
                        }
                    }

                    Console.ReadLine();
#endif
                    break;
            }

            if (_verifyDatabaseStructureSucceeded)
            {
                NhHelper.Singleton.ControlAssemblies(
                    assemblies,
                    ConnectionString.LoadFromRegistry(CgpServerGlobals.REGISTRY_CONNECTION_STRING));
            }

            _databaseVerificationEnded.Set();
        }

        private bool _verifyDatabaseStructureSucceeded;

        private bool WaitForVerifyDatabaseStructureEnd()
        {
            // wait for database verification to end
            _databaseVerificationEnded.WaitOne();
            return _verifyDatabaseStructureSucceeded;
        }

        private static void StartServiceAfterMaintenance()
        {
            if (
                !Dialog.Question(
                    CgpServerGlobals.NOVA_SERVER + " configuration",
                    "Do you want to start " + CgpServerGlobals.NOVA_SERVER +
                    " service ?"))
                return;

            try
            {
                var sc = new ServiceController(CgpServerGlobals.CGP_SERVICE_NAME);

                if (sc.Status == ServiceControllerStatus.Stopped)
                    sc.Start();
            }
            catch
            {
                var logCgpServer = new Log(CgpServerGlobals.NOVA_SERVER, false, true, false);

                logCgpServer.Error("Service not installed");

                Dialog.Error("Service not installed");
            }
        }

        void OnPluginSetLoaded(ACgpServerPlugin[] parameter)
        {
            SafeThread.StartThread(VerifyDatabaseStructure);
        }

        private void UnloadPlugins()
        {
            if (PluginManager != null)
                PluginManager.UnloadAllPlugins();
        }

        private void OnPluginLoaded(ACgpServerPlugin plugin)
        {
            LogCgpServer.Info("Plugin \"" + plugin.CgpDesignation + "\" successfuly loaded");
            plugin.SetLanguage(LocalizationHelper.ActualLanguage);
        }

        private void OnPluginUnlincensed(string description)
        {
            LogCgpServer.Error("Plugin \"" + description + "\" is not licensed plugin");
        }

        private void OnPluginFailed(string description)
        {
            LogCgpServer.Error("Loading plugin \"" + description + "\" failed");
        }

        private void InitRemoting()
        {
            LogCgpServer.Info("Initializing remoting routines");

            SetClientSessionTimeout();

            var aesSettings =
                new AESSettings(
                    QuickHashes.GetSHA256(
                        Encoding.UTF8.GetString(CgpServerGlobals.Singleton.REMOTING_KEY)),
                    CgpServerGlobals.Singleton.REMOTING_SALT);

            if (!TcpUdpPort.IsFreeTcp(
                    GeneralOptions.Singleton.RemotingServerPort,
                    true))
                LogCgpServer.Error(
                    "Port " + GeneralOptions.Singleton.RemotingServerPort +
                    " for initializing remoting routines is not free. Release the port that the client can connect");

            _remotingPeer =
                TcpRemotingPeer.GetServerSide(
                    GeneralOptions.Singleton.RemotingServerIpAddress,
                    GeneralOptions.Singleton.RemotingServerPort,
                    false,
                    aesSettings);

            _remotingPeer.ProvideObject<RemotingCommon.ICgpServerRemotingProvider>(
                CgpServerRemotingProvider.Singleton);

            if (PluginManager != null)
                PluginManager.PluginsProvideObject(ref _remotingPeer);
        }

        private CNMPAgent _cnmpAgent;

        private void InitCNMP()
        {
            bool isFree = TcpUdpPort.IsFreeUdp(34000, true);

            try
            {
                _cnmpAgent = new CNMPAgent(CgpServerGlobals.CNMP_INSTANCE_PREFIX +
                    GeneralOptions.Singleton.FriendlyName, CgpServerGlobals.CNMP_TYPE);

                int tcpPort = GeneralOptions.Singleton.RemotingServerPort;

                if (TcpUdpPort.IsValid(tcpPort, false))
                    _cnmpAgent.SetExtra(
                        CgpServerGlobals.CNMP_SERVER_PORT,
                        tcpPort.ToString(CultureInfo.InvariantCulture));

                if (Validator.IsNotNullString(
                        GeneralOptions.Singleton.FriendlyName))
                    _cnmpAgent.SetExtra(
                        CgpServerGlobals.CGP_SERVER_FRIENDLY_NAME,
                        GeneralOptions.Singleton.FriendlyName);

                _cnmpAgent.SetExtra(
                    CgpServerGlobals.CNMP_SERVER_MACHINE_NAME,
                    Environment.MachineName);

                _cnmpAgent.SetExtra(
                    CgpServerGlobals.CNMP_SERVER_MULTIHOMING,
                    _remotingPeer.Multihomed
                        ? "1"
                        : "0");

                if (Version != null)
                    _cnmpAgent.SetExtra(
                        CgpServerGlobals.CNMP_SERVER_VERSION,
                        Version.ToString());

                string localizeLicenceName;
                object licence;

                CgpServerRemotingProvider.Singleton.GetLicensePropertyInfo(
                    "Edition",
                    out localizeLicenceName,
                    out licence);

                _cnmpAgent.SetExtra(
                    CgpServerGlobals.CNMP_SERVER_EDITION_NAME,
                    licence == null
                        ? "DEMO"
                        : licence.ToString());

                _cnmpAgent.Start();
            }
            catch (Exception ex)
            {
                if (isFree)
                    throw;

                throw new Exception(ex.Message + "\nPort 34000 is not free");
            }
        }

        private bool LoadGeneralConfiguration()
        {
            LogCgpServer.Info("Loading server configuration ...");

            if (GeneralOptions.Singleton.IsConfigured)
                return true;

            LogCgpServer.Info(CgpServerGlobals.NOVA_SERVER + " is not configured");

            bool unused;

            return InvokeGeneralConfiguration(out unused);
        }

        private bool InvokeGeneralConfiguration(out bool skipDbReconfiguration)
        {
            skipDbReconfiguration = false;

            if (RunningAsService)
            {
                LogCgpServer.Error(
                    "General options are not configured and can't be configured under service mode !\n");

                return false;
            }

            if (!Singleton.IsFirst)
                try
                {
                    Process current = Process.GetCurrentProcess();

                    foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                        if (process.Id != current.Id)
                            if (
                                Dialog.Question(
                                    LocalizationHelper.GetString(
                                        "QuestionCloseServer")))
                                process.Kill();
                            else
                            {
                                LogCgpServer.Error(
                                    "Contal nova server is already running!\n");
#if DEBUG
                                Console.ReadLine();
#endif
                                return false;
                            }

                    var sc =
                        new ServiceController(
                            CgpServerGlobals.CGP_SERVICE_NAME);

                    if (sc.Status == ServiceControllerStatus.Running)
                        if (Dialog.Question(
                                LocalizationHelper.GetString(
                                    "QuestionStopService")))
                            sc.Stop();
                        else
                        {
                            LogCgpServer.Error(
                                "Contal nova server is already running!\n");
#if DEBUG
                            Console.ReadLine();
#endif
                            return false;
                        }
                }
                catch
                {
                    LogCgpServer.Error(
                        "Contal nova server is already running!\n");
#if DEBUG
                    Console.ReadLine();
#endif
                    return false;
                }

            var moDir = new MovingOriginDirectories();
            moDir.CopyOriginFiles();

            var generalOptionsForm =
                new GeneralOptionsForm(
                    LocalizationHelper,
                    GeneralOptions.Singleton);

            generalOptionsForm.ShowDialog();

            if (!GeneralOptions.Singleton.IsConfigured)
            {
                const string tmpMessage =
                    "General options are not configured !\n\nServer will close immediately.";

                Dialog.Error(tmpMessage);
                LogCgpServer.Error(tmpMessage);
#if DEBUG

                Console.Write("Press enter to continue ...");
                Console.ReadLine();

#endif
                return false;
            }

            if (generalOptionsForm.SkipDBReconfiguration)
                skipDbReconfiguration = true;

            return true;
        }

        public bool IsConnectedToDatabase { get; private set; }

        private void InitDatabaseDefaults()
        {
            try
            {
                LogCgpServer.Info("Initialization of database defaults");

                try
                {
                    RegistryHelper.GetOrAddKey(
                        @"HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\SpecialAccounts\Userlist\").
                        SetValue("NovaServiceUser", 0);
                }
                catch
                {
                }

                IsConnectedToDatabase = true;

                AlarmsManager.Singleton.ServerAlarmsOwner.LoadServerAlarmsFromDatabase();
                AlarmsManager.Singleton.ChangedAlarm += ChangedAlarm;

                GeneralOptions.Singleton.InsertToDatabaseAndRemoveFromRegistry();
                OnDatabaseCheckTimer(null);
                Logins.Singleton.EnsureDefaultAccess();
                PerformPresentationProcessor.Singleton.RunAllCisNG();
                PerformPresentationProcessor.Singleton.CreateStore();
                TimeAxis.Singleton.StartOrReset();
                DatabaseBackup.Singleton.Start();
                EventlogExpiration.Singleton.Start();
                Eventlogs.Singleton.EnsureNewEventSourceStructure();
                DayTypes.Singleton.CreateDefaultDayTypes();
                Calendars.Singleton.CreateDefaultCalendar();
                CardSystems.Singleton.CreateDefaultCardSystems();
                DailyPlans.Singleton.CreateDefaultDailyPlans();
                AlarmPrioritiesDbs.Singleton.LoadAlarmPrioritiesFromDatabase();

                PluginManager.InitDatabaseDefaults();

                StructuredSubSites.Singleton.CreateGlobalEvaluator();
            }
            catch
            {
            }
        }

        void ChangedAlarm(ServerAlarm serverAlarm, bool wasCreatedOrChangedAlarmState)
        {
            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunChangedAlarm,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                new object[] { serverAlarm.ServerAlarmCore });
        }

        private static void RunChangedAlarm(ARemotingCallbackHandler remoteHandler, object[] data)
        {
            var changeAlarmsHandler = remoteHandler as AlarmStateChangedHandler;

            if (changeAlarmsHandler != null)
                changeAlarmsHandler.RunEvent((ServerAlarmCore)data[0]);
        }

        private const int DATABASECHECKDELAY = 60000;

        private bool _databaseConnected;
        private ITimer _databaseCheckTimer;
        private Eventlog _eventlogDisconDatabase;

        private void RefreshDatabaseCheckTimer(string databaseServer)
        {
            if (_databaseCheckTimer != null)
                _databaseCheckTimer.StopTimer();

            _databaseCheckTimer = TimerManager.Static.StartTimer(
                DATABASECHECKDELAY,
                false,
                OnDatabaseCheckTimer,
                databaseServer);
        }

        private Guid _datebaseNameGuid = Guid.Empty;

        private bool OnDatabaseCheckTimer(TimerCarrier timerCarrier)
        {
            string databaseServer =
                timerCarrier != null
                    ? timerCarrier.Data as string
                    : string.Empty;

            if (string.IsNullOrEmpty(databaseServer))
                databaseServer =
                    ConnectionString
                        .LoadFromRegistry(CgpServerGlobals.REGISTRY_CONNECTION_STRING)
                        .DatabaseServer;

            try
            {
                if (CheckDatabaseServerConnectivity(databaseServer))
                    OnDatabaseConnected();
                else
                    OnDatabaseDisconnected();
            }
            catch
            {
            }

            RefreshDatabaseCheckTimer(databaseServer);
            return true;
        }

        private void OnDatabaseDisconnected()
        {
            if (!_databaseConnected)
                return;

            _databaseConnected = false;

            _eventlogDisconDatabase =
                new Eventlog(
                    Eventlog.TYPEDISCONNECTDATABASE,
                    DateTime.Now,
                    GetType()
                        .Assembly.GetName()
                        .Name,
                    "Database is disconnected");

            var systemEventLog = new EventLog
            {
                Source = "CgpServer"
            };

            try
            {
                systemEventLog.WriteEntry(
                    string.Format(
                        "Database is disconnected CGPSource: {0}, EventSource: Database",
                        GetType().Assembly.GetName().Name),
                    EventLogEntryType.Error);
            }
            catch
            {
            }

            SystemEventPerformer.ReportSystemEventStatic(
                SystemEvent.DATABASE_DISCONNECTED,
                " - Database is disconnected");

            CgpServerRemotingProvider.Singleton
                .ForeachCallbackHandler(
                    RunSqlServerOnlineStateChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[] { false });
        }

        private void OnDatabaseConnected()
        {
            if (_databaseConnected)
                return;

            CentralNameRegisters.Singleton.EnsureFixedEventSources();

            _databaseConnected = true;

            if (_eventlogDisconDatabase != null)
            {
                var eventlogInsertItem =
                    new EventlogInsertItem(
                        _eventlogDisconDatabase,
                        Enumerable.Repeat(
                        new EventSource(
                            _datebaseNameGuid,
                            _eventlogDisconDatabase),
                        1),
                        null);

                EventlogInsertQueue.Singleton.Enqueue(eventlogInsertItem);
            }

            _datebaseNameGuid =
                CentralNameRegisters.Singleton
                    .GetGuidFromName(
                        CentralNameRegister.IMPLICIT_CN_DATABASE_NAME);

            Eventlogs.Singleton.InsertEvent(
                Eventlog.TYPECONNECTDATABASE,
                GetType()
                    .Assembly.GetName()
                    .Name,
                new[] { _datebaseNameGuid },
                "Database is connected");

            SystemEventPerformer.ReportSystemEventStatic(
                SystemEvent.DATABASE_DISCONNECTED,
                " - Database is connected");

            CgpServerRemotingProvider.Singleton
                .ForeachCallbackHandler(
                    RunSqlServerOnlineStateChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[] { true });
        }

        public void RunSqlServerOnlineStateChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] sqlServerObj)
        {
            if (sqlServerObj == null)
                return;

            var sqlServerOnlineStateChangedHandler =
                remoteHandler as SqlServerOnlineStateChangedHandler;

            if (sqlServerOnlineStateChangedHandler != null)
                sqlServerOnlineStateChangedHandler.RunEvent((bool)sqlServerObj[0]);
        }

        private static bool CheckDatabaseServerConnectivity(string databaseServer)
        {
            string hostName = databaseServer;
            string serviceName = "MSSQLSERVER";

            int pos = hostName.IndexOf('\\');

            if (pos > 0)
            {
                hostName = hostName.Substring(0, pos);
                serviceName = "MSSQL$" + databaseServer.Substring(hostName.Length + 1);
            }

            if (string.Equals(
                    hostName,
                    Dns.GetHostName(),
                    StringComparison.CurrentCultureIgnoreCase))
                return
                    ServiceController.GetServices()
                        .Where(s => s.ServiceName == serviceName)
                        .All(s => s.Status == ServiceControllerStatus.Running);

            var client = new TcpClient();

            try
            {
                client.Connect(hostName, 1433);
                client.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }
        
        // SB
        private void StartConsoleHandler()
        {
            #if DEBUG
            Console.WriteLine("PRESS Ctrl & Break TO EXIT ...");
            Console.CancelKeyPress += (sender, eventArgs) => RequestStop();
            #else
            Console.WriteLine("INFINITE RELEASE LOOP");
            #endif
        }

        /* SB
        private void StartConsoleHandler()
        {
            SafeThread.StartThread(ConsoleHandlerThread);
        }

        private void ConsoleHandlerThread()
        {
            #if DEBUG
            Console.WriteLine("PRESS ANY KEY TO EXIT ...");
            Console.ReadLine();
            RequestStop();
            #else
            Console.WriteLine("INFINITE RELEASE LOOP");
            #endif
        }
        */

        /// <summary>
        /// core routine of the CGP server to be executed
        /// </summary>
        protected override void ProcessingThread()
        {
            try
            {
                SimpleAbout();

                TimerManager.Static.StartTimeout(600000, OnTimeoutCheckThreadCount);

                bool skipDbReconfiguration = false;

                switch (GeneralMode)
                {
                    case GeneralMode.Runtime:
                        if (!LoadGeneralConfiguration())
                            return;

                        break;

                    case GeneralMode.Maintenance:
                        if (!InvokeGeneralConfiguration(out skipDbReconfiguration))
                            return;

                        break;

                    case GeneralMode.Verification:
                        break;
                }

                if (!skipDbReconfiguration)
                {
                    LoadPlugins();

                    if (GeneralMode == GeneralMode.Runtime)
                        LoadAndProcessLicenseProperties();

                    EnsureUpgradeDirectories();

                    if (!WaitForVerifyDatabaseStructureEnd())
                    {
                        LogCgpServer.Error("Update the database failed");
#if DEBUG
                        Console.ReadLine();
#endif
                        return;
                    }
                }

                switch (GeneralMode)
                {
                    case GeneralMode.Runtime:
                        InitDatabaseDefaults();
                        break;
                }

                SetDHCP();

                //automatically add NTP server ip addresses 
                // if a NTP server is running on PC as Nova server
                AddNTPIps(!skipDbReconfiguration);

                switch (GeneralMode)
                {
                    case GeneralMode.Maintenance:

                        StartServiceAfterMaintenance();
                        break;

                    case GeneralMode.Runtime:

                        InitRemoting();

                        InitCNMP();

                        if (!RunningAsService)
                            StartConsoleHandler();

                        SafeThread.StartThread(ProcessUniqueAKeyCSRestriction);

                        BlockUntilStopped();
                        break;
                }
            }
            catch (ThreadAbortException)
            {
                UnloadPlugins();
            }
            catch (Exception error)
            {
                LogCgpServer.Error("Failed with error :\n" + error.Message);
#if DEBUG
                Console.ReadLine();
#endif
            }
            finally
            {
                UnloadPlugins();
            }
        }

        private static void ProcessUniqueAKeyCSRestriction()
        {
            //unique A key verification if required
            if (!GeneralOptions.Singleton.UniqueAKeyCSRestriction)
                return;

            ICollection<MifareSectorData> sectorsData;

            if ((sectorsData = CardSystems.Singleton.ListMifareSectorSmartData()) == null)
                return;

            //MifareCardData id is same as CardSystem it belongs to
            var cardSystemsUsingAKey = new Dictionary<string, HashSet<Guid>>();

            foreach (MifareSectorData sectorData in sectorsData)
            {
                if (sectorData.SectorsInfo == null)
                    continue;

                var currentSectorData = sectorData;

                var hexaForms =
                    sectorData.SectorsInfo
                        .Where(sectorInfo => sectorInfo.Bank != null)
                        .Select(
                            sectorInfo => ByteDataCarrier.HexDump(
                                sectorInfo.InheritAKey
                                    ? currentSectorData.GeneralAKey
                                    : sectorInfo.AKey));

                foreach (string hexaForm in hexaForms)
                {
                    HashSet<Guid> cardSystems;

                    if (!cardSystemsUsingAKey.TryGetValue(hexaForm, out cardSystems))
                    {
                        cardSystems = new HashSet<Guid>();
                        cardSystemsUsingAKey.Add(hexaForm, cardSystems);
                    }

                    cardSystems.Add(sectorData.Id);
                }
            }

            var alarmParentObjectsLists =
                cardSystemsUsingAKey.Values
                    .Where(ids => ids.Count > 1)
                    .Select(
                        conflictedIds =>
                            conflictedIds
                                .Select(
                                    id =>
                                        new IdAndObjectType(
                                            id,
                                            ObjectType.CardSystem))
                                .ToList());

            int index = 1;

            foreach (var alarmParentObjectsList in alarmParentObjectsLists)
                AlarmsManager.Singleton.AddAlarm(
                    new ServerAlarm(
                        new ServerAlarmCore(
                            new Alarm(
                                new AlarmKey(
                                    AlarmType.GeneralAlarm,
                                    null),
                                AlarmState.Normal),
                            alarmParentObjectsList,
                            string.Format("Mifare sector card systems A keys collision {0}", index++),
                            "Contal Nova Server",
                            "Mifare sector card systems are using same A key")));
        }

        private bool OnTimeoutCheckThreadCount(TimerCarrier timer)
        {
            try
            {
                var eventLogStringBuilder = new StringBuilder();

                int threadCount = 0;

                try
                {
                    threadCount =
                        Process.GetCurrentProcess().Threads.Count;

                    eventLogStringBuilder.AppendFormat(
                        "Threads count: {0}",
                        threadCount);
                }
                catch
                {
                    eventLogStringBuilder.Append("Get threads count failed");
                }

                if (threadCount > 20)
                    return true;

                eventLogStringBuilder.AppendLine();

                try
                {
                    foreach (ProcessThread pt in Process.GetCurrentProcess().Threads)
                    {
                        eventLogStringBuilder.AppendLine();
                        eventLogStringBuilder.AppendFormat(
                            "Thread id: {0}",
                            pt.Id);

                        eventLogStringBuilder.AppendLine();
                        eventLogStringBuilder.AppendFormat(
                            "Thread state: {0}",
                            pt.ThreadState);

                        eventLogStringBuilder.AppendLine();
                        eventLogStringBuilder.AppendFormat(
                            "Start time: {0}",
                            pt.StartTime);

                        eventLogStringBuilder.AppendLine();
                        eventLogStringBuilder.AppendFormat(
                            "User processor time: {0}",
                            pt.UserProcessorTime);

                        eventLogStringBuilder.AppendLine();
                        eventLogStringBuilder.AppendFormat(
                            "Wait reason: {0}",
                            pt.WaitReason);
                    }
                }
                catch
                {
                    eventLogStringBuilder.Append(
                        "Get threads information failed");
                }

                LogCgpServer.Info(eventLogStringBuilder.ToString());
            }
            catch
            {
            }

            return true;
        }

        private static void AddNTPIps(bool intoDb)
        {
            try
            {
                if ((int)RegistryHelper
                        .GetOrAddKey(
                            @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\W32Time\TimeProviders\NtpServer",
                            false)
                        .GetValue("Enabled") != 1)
                    return;

                //Check if port is open

                if (System.Net.NetworkInformation.IPGlobalProperties
                        .GetIPGlobalProperties()
                        .GetActiveUdpListeners()
                        .Count(p => p.Port == 123) != 1)
                    return;

                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

                var ipAddresses = new StringBuilder();

                foreach (IPAddress ip in host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork))
                {
                    ipAddresses.Append(ip);
                    ipAddresses.Append(StringConstants.SEMICOLON);
                }

                if (!intoDb)
                    return;

                if (ipAddresses.Length <= 0)
                    return;

                ipAddresses.Remove(ipAddresses.Length - 1, 1);
                ServerGeneralOptionsDBs.Singleton.AddNtpIpAddress(ipAddresses.ToString());
            }
            catch
            {
            }
        }

        private void EnsureUpgradeDirectories()
        {
            QuickPath.EnsureDirectory(ClientUpgradesPath);
            PluginManager.EnsureUpgradeDirectories();
        }

        private void SetDHCP()
        {
            AutoStartDHCP = GeneralOptions.Singleton.AutoStartDHCP;

            if (AutoStartDHCP)
                StartDHCP(true);
        }

        protected override void Preprocessing()
        {
            LogCgpServer = new Log(CgpServerGlobals.NOVA_SERVER, !RunningAsService, true, false);
        }

        private static void OnSessionTimeOut(object sessionId)
        {
            CentralTransactionControl.Singleton.SessionTimeOut(sessionId);
            CgpServerRemotingProvider.Singleton.UnauthenticateSessionTimeOut(sessionId);
        }

        // Alarms queue
        public void CreateAlarmsQueue()
        {
            var alarmsManager = AlarmsManager.Singleton;

            alarmsManager.SetDelegateChangeAlarm(RunAddAlarm);
            alarmsManager.RemovedAlarm += RunDeletedAlarm;

            alarmsManager.RegisterAlarmsOwner(new ServerAlarmsOwner());
        }

        public void CreateAlarmEventlog(
            bool alarmOccured,
            DateTime dateTime,
            AlarmState alarmState,
            bool isAkcnwledged,
            bool isBlocked,
            bool appendUserNameParameter,
            string name,
            string parentObject,
            string cgpSource,
            IEnumerable<IdAndObjectType> relatedObjects,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            string description;
            string eventlogType;

            if (alarmOccured)
            {
                description = string.Format(
                    "Alarm {0} was added",
                    name);

                eventlogType = Eventlog.TYPEALARMOCCURED;
            }
            else if (isAkcnwledged
                     && alarmState == AlarmState.Normal
                     && !isBlocked)
            {
                description = string.Format(
                    "Alarm {0} was removed",
                    name);

                eventlogType = Eventlog.TYPEINACTALARMACKNOWLEDGED;
            }
            else
            {
                description = string.Format(
                    "Alarm {0} changed to {1}/{2}/{3}",
                    name,
                    alarmState,
                    GetAlarmAcknowledgedStateString(isAkcnwledged),
                    GetAlarmBlockingStateString(isBlocked));

                eventlogType = Eventlog.TYPEACTALARMACKNOWLEDGED;
            }

            var newEventlog = new Eventlog(
                eventlogType,
                dateTime,
                cgpSource,
                description);

            eventlog = newEventlog;

            eventlogParameters = new List<EventlogParameter>
            {
                new EventlogParameter(
                    "Created",
                    dateTime.ToString("dd.MM.yyyy HH:mm:ss")),
                new EventlogParameter(
                    "Parent object",
                    parentObject),
                new EventlogParameter(
                    "Unique name",
                    name),
                new EventlogParameter(
                    "Alarm state",
                    string.Format(
                        "{0}/{1}/{2}",
                        alarmState,
                        GetAlarmAcknowledgedStateString(isAkcnwledged),
                        GetAlarmBlockingStateString(isBlocked))),
                new EventlogParameter(
                    "Description",
                    description)
            };

            if (appendUserNameParameter)
            {
                string sessionId =
                    ClientIdentificationServerSink.CallingSessionId;

                if (sessionId != null)
                {
                    RemotingSession rs =
                        RemotingSessionHandler.Singleton[sessionId];
                    var parameters =
                        rs[RemotingSession.SESSIONPARAMETERS] as
                            LoginAuthenticationParameters;

                    if (parameters != null)
                        eventlogParameters.Add(
                            new EventlogParameter(
                                "UserName",
                                parameters.LoginUsername));
                }
            }

            eventSources =
                relatedObjects != null
                    ? new List<EventSource>(
                        relatedObjects.Select(
                            relatedObject =>
                                new EventSource(
                                    (Guid)relatedObject.Id,
                                    newEventlog)))
                    : null;
        }

        public void WriteToEventlogAlarmOccured(ServerAlarm serverAlarm)
        {
            Eventlog eventlog;
            List<EventSource> eventSources;
            List<EventlogParameter> eventlogParameters;

            var serverAlarmProxy = serverAlarm.ServerAlarmCore;

            CreateAlarmEventlog(
                true,
                serverAlarmProxy.Alarm.CreatedDateTime,
                serverAlarmProxy.Alarm.AlarmState,
                serverAlarmProxy.Alarm.IsAcknowledged,
                serverAlarmProxy.Alarm.IsBlocked,
                false,
                serverAlarmProxy.Name,
                serverAlarmProxy.ParentObject,
                GetType().Assembly.GetName().Name,
                serverAlarmProxy.RelatedObjects,
                out eventlog,
                out eventSources,
                out eventlogParameters);

            EventlogInsertQueue.Singleton.Enqueue(
                new EventlogInsertItem(
                    eventlog,
                    eventSources,
                    eventlogParameters));
        }

        public void WriteToEventlogAlarmChanged(ServerAlarm serverAlarm)
        {
            Eventlog eventlog;
            List<EventSource> eventSources;
            List<EventlogParameter> eventlogParameters;

            var serverAlarmProxy = serverAlarm.ServerAlarmCore;

            CreateAlarmEventlog(
                true,
                serverAlarmProxy.Alarm.CreatedDateTime,
                serverAlarmProxy.Alarm.AlarmState,
                serverAlarmProxy.Alarm.IsAcknowledged,
                serverAlarmProxy.Alarm.IsBlocked,
                true,
                serverAlarmProxy.Name,
                serverAlarmProxy.ParentObject,
                GetType().Assembly.GetName().Name,
                serverAlarmProxy.RelatedObjects,
                out eventlog,
                out eventSources,
                out eventlogParameters);

            EventlogInsertQueue.Singleton.Enqueue(
                new EventlogInsertItem(
                    eventlog,
                    eventSources,
                    eventlogParameters));
        }

        private static string GetAlarmAcknowledgedStateString(bool isAcknowledged)
        {
            return isAcknowledged
                ? "Acknowledged"
                : "Not acknowledged";
        }

        private static string GetAlarmBlockingStateString(bool isBlocked)
        {
            return isBlocked
                ? "Blocked"
                : "Unblocked";
        }

        private static void RunAddAlarm()
        {
            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunAddAlarm,
                DelegateSequenceBlockingMode.Asynchronous,
                false);
        }

        private static void RunAddAlarm(ARemotingCallbackHandler remoteHandler)
        {
            var changeAlarmsHandler = remoteHandler as ChangeAlarmsHandler;

            if (changeAlarmsHandler != null)
                changeAlarmsHandler.RunEvent();
        }

        private static void RunDeletedAlarm(IdServerAlarm deleted)
        {
            try
            {
                CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    RunDeletedAlarm,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new[] { (object)deleted });
            }
            catch
            { }
        }

        private static void RunDeletedAlarm(
            ARemotingCallbackHandler remoteHandler,
            object[] obj)
        {
            if (obj == null || obj.Length != 1)
                return;

            var changeAlarmsHandler = remoteHandler as ChangeAlarmsHandler;

            if (changeAlarmsHandler != null)
                changeAlarmsHandler.RunDeletedAlarm((IdServerAlarm)obj[0]);
        }

        public bool ClientUpgradeAvailable(Version callerVersion, out Version newVersion)
        {
            IEnumerable<Version> versions;

            if (QuickPath.EnsureDirectory(ClientUpgradesPath))
            {
                Exception ex;

                versions =
                    Directory
                        .GetFiles(ClientUpgradesPath)
                        .Select(
                            file =>
                                FilePacker.TryGetHeaderParameters(
                                    file,
                                    out ex))
                        .Where(
                            headerParameters =>
                                headerParameters != null &&
                                headerParameters.Length >= 4 &&
                                headerParameters[0].ToLower() == "client")
                        .Select(
                            headerParameters =>
                                new Version(headerParameters[1]));
            }
            else
                versions = Enumerable.Empty<Version>();

            newVersion = versions.DefaultIfEmpty(callerVersion).Max();

            return newVersion.CompareTo(callerVersion) > 0;
        }

        public long GetClientUpgradeFileLength(Version upgradeVersion)
        {
            if (!EnsureOpenUpgradeStream(upgradeVersion))
                return 0;

            long result = _openedClientUpgradeFiles[upgradeVersion].Length;

            try
            {
                _openedClientUpgradeFiles[upgradeVersion].Close();
            }
            catch
            {
            }

            _openedClientUpgradeFiles.Remove(upgradeVersion);

            return result;
        }

        private const int ClientUpgradeFileBufferLength = 32768;

        private readonly Dictionary<Version, FileStream> _openedClientUpgradeFiles =
            new Dictionary<
                Version,
                FileStream>();

        public byte[] GetClientUpgradeData(
            Version upgradeVersion,
            int fromFilePosition)
        {
            lock (_openedClientUpgradeFiles)
            {
                if (!EnsureOpenUpgradeStream(upgradeVersion))
                    return null;

                _openedClientUpgradeFiles[upgradeVersion].Position = fromFilePosition;

                var fileBytes = new byte[ClientUpgradeFileBufferLength];

                int dataFromFileLength =
                    _openedClientUpgradeFiles[upgradeVersion].Read(
                        fileBytes,
                        0,
                        ClientUpgradeFileBufferLength);

                try
                {
                    _openedClientUpgradeFiles[upgradeVersion].Close();
                }
                catch
                {
                }
                _openedClientUpgradeFiles.Remove(upgradeVersion);

                var result = new byte[dataFromFileLength];

                Buffer.BlockCopy(
                    fileBytes,
                    0,
                    result,
                    0,
                    result.Length);

                return result;
            }
        }

        private bool EnsureOpenUpgradeStream(Version upgradeVersion)
        {
            if (_openedClientUpgradeFiles.ContainsKey(upgradeVersion))
                return true;

            if (!QuickPath.EnsureDirectory(ClientUpgradesPath))
                return false;

            foreach (string file in Directory.GetFiles(ClientUpgradesPath))
            {
                FileStream fs = null;

                try
                {
                    string[] header = FilePacker.GetHeaderParameters(file);

                    if (header == null || header.Length < 2)
                        continue;

                    if (header[1].ToLower() != upgradeVersion.ToString().ToLower())
                        continue;

                    fs =
                        new FileStream(
                            file,
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.Read,
                            ClientUpgradeFileBufferLength);

                    _openedClientUpgradeFiles.Add(upgradeVersion, fs);

                    return true;
                }
                catch (Exception)
                {
                    if (fs == null)
                        continue;

                    try
                    {
                        fs.Close();
                    }
                    catch
                    {
                    }
                }
            }

            return true;
        }

        private FileStream _lastEventLogReportFile;

        public byte[] GetLastEventlogReport(int fromFilePosition)
        {
            //  lock (_lastEventLogReportFile)
            {
                if (!EnsureOpenLastReportStream())
                    return null;

                _lastEventLogReportFile.Position = fromFilePosition;

                var fileBytes = new byte[ClientUpgradeFileBufferLength];

                int dataFromFileLength = _lastEventLogReportFile.Read(fileBytes, 0, ClientUpgradeFileBufferLength);

                try
                {
                    _lastEventLogReportFile.Close();
                }
                catch
                {
                }
                _lastEventLogReportFile = null;

                var result = new byte[dataFromFileLength];

                Buffer.BlockCopy(fileBytes, 0, result, 0, result.Length);

                return result;
            }
        }

        private bool EnsureOpenLastReportStream()
        {
            if (_lastEventLogReportFile != null)
                return true;

            FileStream fs = null;

            try
            {
                fs = new FileStream(Eventlogs.Singleton.LastReportFile, FileMode.Open, FileAccess.Read, FileShare.Read, ClientUpgradeFileBufferLength);

                _lastEventLogReportFile = fs;

                return true;
            }
            catch (Exception)
            {
                try
                {
                    fs.Close();
                }
                catch
                {
                }
            }

            return true;
        }

        public bool IsLastEventlogReportAvailable()
        {
            if (String.IsNullOrEmpty(Eventlogs.Singleton.LastReportFile))
                return false;

            return File.Exists(Eventlogs.Singleton.LastReportFile);
        }

        public void CleanLastEventlogReport()
        {
            try
            {
                File.Delete(Eventlogs.Singleton.LastReportFile);
                Eventlogs.Singleton.LastReportFile = null;
            }
            catch (Exception)
            {

            }
        }

        public bool GetLicencePropertyInfoFromServerAndPlugins(
            string propertyName,
            out string localisedName,
            out object value)
        {
            return GetLicencePropertyInfoFromServerAndPlugins(
                propertyName,
                null,
                out localisedName,
                out value);
        }

        public bool GetLicencePropertyInfoFromServerAndPlugins(
            string propertyName,
            string language,
            out string localisedName,
            out object value)
        {
            return
                GetLicencePropertyInfo(
                    propertyName,
                    language,
                    out localisedName,
                    out value) ||
                GetLicencePropertyInfoFromPlugins(
                    propertyName,
                    language,
                    out localisedName,
                    out value);
        }

        public bool GetLicencePropertyInfoFromPlugins(
            string propertyName,
            out string localisedName,
            out object value)
        {
            return GetLicencePropertyInfoFromPlugins(
                propertyName,
                null,
                out localisedName,
                out value);
        }

        public bool GetLicencePropertyInfoFromPlugins(
            string propertyName,
            string language,
            out string localisedName,
            out object value)
        {
            localisedName = string.Empty;
            value = null;

            if (PluginManager == null)
                return false;

            foreach (ACgpServerPlugin plugin in PluginManager.GetLoadedPlugins())
                if (plugin.GetLicencePropertyInfo(
                        propertyName,
                        language,
                        out localisedName,
                        out value))
                    return true;

            return false;
        }

        public bool GetLicencePropertyInfo(
            string propertyName,
            out string localisedName,
            out object value)
        {
            return
                GetLicencePropertyInfo(
                    propertyName,
                    GeneralOptions.Singleton.CurrentLanguage,
                    out localisedName,
                    out value);
        }

        public bool GetLicencePropertyInfo(
            string propertyName,
            string language,
            out string localisedName,
            out object value)
        {
            if (string.IsNullOrEmpty(language))
                language = GeneralOptions.Singleton.CurrentLanguage;

            localisedName = propertyName;

            if (!_requiredLicenceProperties.TryGetValue(propertyName, out value))
                return false;

            if (value == null)
                return true;

            // this part is not that important,
            // so even if translation fails, the value cannot be degraded by any exception
            try
            {
                localisedName =
                    LocalizationHelper.GetString(
                        propertyName,
                        language);
            }
            catch
            {
                try
                {
                    localisedName =
                        LocalizationHelper.GetString(
                            propertyName,
                            CgpServerGlobals.DEFAULT_LANGUAGE);
                }
                catch
                {
                    localisedName = propertyName;
                }
            }

            return true;
        }

        private SafeThread _thread;

        internal void StartTestingThread()
        {
            _thread = new SafeThread(DoTest);
            _thread.Start();
        }

        private readonly Random _rand = new Random();

        private void DoTest()
        {
            try
            {
                while (true)
                {
                    PluginManager.RunDBTest();
                    Thread.Sleep(_rand.Next(10000) + 2000);
                }
            }
            catch
            {
            }
        }

        internal void StopTestingThred()
        {
            if (_thread == null || !_thread.IsStarted)
                return;

            try
            {
                _thread.Stop(5);
                _thread = null;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Get client session timeout from general options and set session timeout
        /// </summary>
        private static void SetClientSessionTimeout()
        {
            int clientSessionTimeoutFromGeneralOptions =
                GeneralOptions.Singleton.ClientSessionTimeOut * 60000;

            //if (RemotingSessionHandler.Singleton.SessionTimeout != clientSessionTimeoutFromGeneralOptions)
            RemotingSessionHandler.Singleton.SessionTimeout = clientSessionTimeoutFromGeneralOptions;
        }

        public void GeneralOptionChanged()
        {
            SafeThread.StartThread(DoGeneralOptionChanged);
        }

        private void DoGeneralOptionChanged()
        {
            SetClientSessionTimeout();

            foreach (ACgpServerPlugin plugin in PluginManager.GetLoadedPlugins())
                plugin.GeneralOptionsChanged();
        }

        public void SetServerAndPluginsLanguage(string language)
        {
            TrySetLanguage(language);

            foreach (ACgpServerPlugin plugin in PluginManager.GetLoadedPlugins())
                plugin.SetLanguage(language);
        }

        public void WarningSetMaxEventsCount(List<string> ccuNames)
        {
            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunWarningSetMaxEventsCount,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                new object[] { ccuNames });
        }

        public void RunWarningSetMaxEventsCount(
            ARemotingCallbackHandler remoteHandler,
            object[] obj)
        {
            var warningSetMaxEventsCountHandler =
                remoteHandler as WarningSetMaxEventsCountHandler;

            if (warningSetMaxEventsCountHandler != null)
                warningSetMaxEventsCountHandler.RunEvent((List<string>)obj[0]);
        }

        public void DemoPeriodHasExpired()
        {
            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunDemoPeriodHasExpired,
                DelegateSequenceBlockingMode.Asynchronous,
                false);
        }

        public void RunDemoPeriodHasExpired(
            ARemotingCallbackHandler remoteHandler)
        {
            var demoPerionHasExpiredHandler =
                remoteHandler as DemoPerionHasExpiredHandler;

            if (demoPerionHasExpiredHandler != null)
                demoPerionHasExpiredHandler.RunEvent();
        }
    }
}
