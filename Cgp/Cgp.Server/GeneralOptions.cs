using System;
using System.Collections.Generic;
using System.Linq;

using Contal.IwQuick.Sys.Microsoft;
using Contal.IwQuick;
using Microsoft.Win32;
using System.Drawing;
using Contal.Cgp.Globals;
using System.IO;
using System.Diagnostics;

using Contal.Cgp.Server.DB;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Server
{
    /// <summary>
    /// general options of the CGP server;
    /// this class should not be instantiated as Singleton, because it 
    /// </summary>
    public class GeneralOptions
    {
        private int _remotingServerPort = CgpServerGlobals.DEFAULT_REMOTING_SERVER_PORT;
        private string _remotingServerIpAddress = string.Empty;
        private string _friendlyName = string.Empty;
        private string _licencePath = string.Empty;
        private bool _autoStartDHCP;
        private bool _isConfigured;
        private bool _registryAccessible = true;
        private Exception _registryAccessException;

        //SMTP Server settings
        private string _smtpServer;
        private int _smtpPort;
        private string _smtpSourceEmailAddress;
        private string _smtpSubject;
        private string _smtpCredentials;
        private bool _smtpSsl;

        //Serial Port settings
        private string _serialPort;
        private int _serialPortBaudRate;
        private int _serialPortDataBits;
        private string _serailPortParity;
        private string _serialPortStopBits;
        private string _serialPortFlowControl;
        private bool _serialPortParityCheck;
        private bool _serialPortCarrierDetect;
        private int? _serialPortPin;

        //Database settings
        private string _timeZoneGuidString;
        private string _databaseBackupPath;
        private int _eventlogsExpirationDays;
        private int _eventlogsMaxCountValue;
        private int _eventlogsMaxCountExponent;
        private string _eventlogTimeZoneGuidString;

        //UI - Color Settings 
        private Color _dragDropColorText;
        private Color _dragDropColorBackground;
        private Color _referenceObjectColorText;
        private Color _referenceObjectColorBackground;
        private Color _alarmNotAcknowledgedColorText;
        private Color _alarmNotAcknowledgedColorBackground;
        private Color _alarmColorText;
        private Color _alarmColorBackground;
        private Color _normalNotAcknowledgedColorText;
        private Color _normalNotAcknowledgedColorBackground;
        private Color _normalColorText;
        private Color _normalColorBackground;
        private Color _noAlarmsInQueueColorText;
        private Color _noAlarmsInQueueColorBackground;

        //UI - Autoclose Settings 
        private bool _isTurnedOn;
        private int _autoCloseTimeout;

        //server localization
        private string _currentLanguage = CgpServerGlobals.DEFAULT_LANGUAGE;

        //Security Setting 
        private int _changePassDays;
        private bool? _requirePINCardLogin;
        private bool _uniqueAndNotNullPersonalKey;
        private bool _lockClientApplication;
        private bool _ccuConfigurationToServerByPassword;
        private bool _requiredSecurePin;
        private bool _disableCcuPnPAutomaticAssignmnet;
        private bool _listOnlyUnassignedCardsInPersonForm;
        private int _delayToSaveAlarmsFromCardReaders;
        private bool _uniqueAKeyCSRestriction;
        private bool _cardReadersAllowPINCachingInMenu;
        private int _minimalCodeLength;
        private int _maximalCodeLength;
        private bool _isPinConfirmationObligatory;

        //Eventlogs
        private bool _eventlogInputStateChanged;
        private bool _eventlogOutputStateChanged;
        private bool _eventlogAlarmAreaAlarmStateChanged;
        private bool _eventlogAlarmAreaActivationStateChanged;
        private bool _eventlogCardReaderOnlineStateChanged;
        private bool _eventSourcesReverseOrder;
        private string _eventlogReportsTimeZoneGuidString;
        private string _eventlogReportsEmails;

        //Advanced access settings
        private bool _enableLoggingSDPSTZChanges;
        private bool _syncingTimeFromServer;
        private int _periodOfTimeSyncWithoutStratum;
        private int _periodicTimeSyncTolerance;
        public bool _alarmAreaRestrictivePolicyForTimeBuying;

        //Advanced settings
        private int _maxEventsCountForInsert;
        private int _delayForSaveEvents;
        private int _clientSessionTimeOut;
        private int _alarmListSuspendedRefreshTimeout;
        private bool _correctDeserializationFailures;
        private int _delayForSendingChangesToCcu;

        //Customer & Supplier info 
        public string CustomerCompanyName { get; set; }
        public string CustomerDeliveryAddress { get; set; }
        public string CustomerZipCode { get; set; }
        public string CustomerCityState { get; set; }
        public string CustomerCountry { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerWebsite { get; set; }
        public string CustomerContactPerson { get; set; }
        public string SupplierCompanyName { get; set; }
        public string SupplierDeliveryAddress { get; set; }
        public string SupplierZipCode { get; set; }
        public string SupplierCityState { get; set; }
        public string SupplierCountry { get; set; }
        public string SupplierPhone { get; set; }
        public string SupplierWebsite { get; set; }
        public string SupplierContactPerson { get; set; }

        //Hidden features
        private IEnumerable<string> _showHiddenFeatures;

        private static volatile GeneralOptions _singleton;
        private static readonly object _syncRoot = new object();

        private bool _isLoadedFromDatabase;
        private readonly object _syncRootLoadFromDatabase = new object();

        public static GeneralOptions Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                        if (_singleton == null)
                            _singleton = new GeneralOptions();

                _singleton.TryLoadFromDatabase();

                return _singleton;
            }
        }

        private GeneralOptions()
        {
            SaveRegistryTimeStamp(CgpServerGlobals.REGISTRY_GENERAL_SETTINGS);
            RunRegistryChanger();
            LoadFromRegistry();
        }

        private static void SaveRegistryTimeStamp(string keyPath)
        {
            try
            {
                RegistryKey key;
                key = RegistryHelper.GetOrAddKey(keyPath);

                key.SetValue(CgpServerGlobals.TIMESTAMP_VALUE_NAME, DateTime.Now.ToUniversalTime().ToString());
                key.Close();
            }
            catch { }
        }

        private static void RunRegistryChanger()
        {
            string registryChangerPath =
                Path.Combine(
                    Environment.CurrentDirectory,
                    @"RegistryChanger\RegistryChanger.exe");

            if (!File.Exists(registryChangerPath))
                return;

            try
            {
                var p = new Process
                {
                    StartInfo =
                    {
                        FileName = registryChangerPath,
                        WorkingDirectory = Path.GetDirectoryName(registryChangerPath),
                        UseShellExecute = true,
                        Verb = "runas",
                        Arguments =
                            string.Join(
                                " ",
                                new[]
                                {
                                    "\"" + CgpServerGlobals.REGISTRY_GENERAL_SETTINGS + "\"",
                                    "\"" + CgpServerGlobals.REGISTRY_GENERAL_SETTINGS_WOW6432NODE + "\"",
                                    "\"" + CgpServerGlobals.TIMESTAMP_VALUE_NAME + "\""
                                })
                    }
                };

                p.Start();
            }
            catch { }
        }

        #region Properties
        /// <summary>
        /// Port for remoting server
        /// </summary>
        public int RemotingServerPort
        {
            get { return _remotingServerPort; }
            set { _remotingServerPort = value; }
        }

        /// <summary>
        /// Current localization language
        /// </summary>
        public string CurrentLanguage
        {
            get { return _currentLanguage; }
            set { _currentLanguage = value; }
        }

        /// <summary>
        /// Ip address of remoting server
        /// </summary>
        public string RemotingServerIpAddress
        {
            get { return _remotingServerIpAddress; }
            set { _remotingServerIpAddress = value; }
        }

        /// <summary>
        /// Friendly name for remoting server
        /// </summary>
        public string FriendlyName
        {
            get { return _friendlyName; }
            set { _friendlyName = value; }
        }

        /// <summary>
        /// Licence file path
        /// </summary>
        public string LicencePath
        {
            get { return _licencePath; }
            set { _licencePath = value; }
        }

        /// <summary>
        /// Gets or sets value indicating, if DHCP server should be started at startup
        /// </summary>
        public bool AutoStartDHCP
        {
            get { return _autoStartDHCP; }
            set { _autoStartDHCP = value; }
        }

        /// <summary>
        /// Is configured corectly
        /// </summary>
        public bool IsConfigured
        {
            get { return _isConfigured; }
        }

        public string SmtpServer
        {
            get { return _smtpServer; }
            set { _smtpServer = value; }
        }

        public int SmtpPort
        {
            get { return _smtpPort; }
            set { _smtpPort = value; }
        }

        public Exception RegistryAccessException
        {
            get { return _registryAccessException; }
        }

        public bool RegistryAccessible
        {
            get { return _registryAccessible; }
        }

        public string SmtpSourceEmailAddress
        {
            get { return _smtpSourceEmailAddress; }
            set { _smtpSourceEmailAddress = value; }
        }
        public string SmtpSubject
        {
            get { return _smtpSubject; }
            set { _smtpSubject = value; }
        }

        public string SmtpCredentials
        {
            get { return _smtpCredentials; }
            set { _smtpCredentials = value; }
        }

        public bool SmtpSsl
        {
            get { return _smtpSsl; }
            set { _smtpSsl = value; }
        }

        public string SerialPort
        {
            get { return _serialPort; }
            set { _serialPort = value; }
        }
        public int SerialPortBaudRate
        {
            get { return _serialPortBaudRate; }
            set { _serialPortBaudRate = value; }
        }
        public int SerialPortDataBits
        {
            get { return _serialPortDataBits; }
            set { _serialPortDataBits = value; }
        }
        public string SerialPortParity
        {
            get { return _serailPortParity; }
            set { _serailPortParity = value; }
        }
        public string SerialPortStopBits
        {
            get { return _serialPortStopBits; }
            set { _serialPortStopBits = value; }
        }
        public string SerialPortFlowControl
        {
            get { return _serialPortFlowControl; }
            set { _serialPortFlowControl = value; }
        }
        public bool SerialPortParityCheck
        {
            get { return _serialPortParityCheck; }
            set { _serialPortParityCheck = value; }
        }
        public bool SerialPortCarrierDetect
        {
            get { return _serialPortCarrierDetect; }
            set { _serialPortCarrierDetect = value; }
        }
        public int? SerialPortPin
        {
            get { return _serialPortPin; }
            set { _serialPortPin = value; }
        }

        public string TimeZoneGuidString
        {
            get { return _timeZoneGuidString; }
            set { _timeZoneGuidString = value; }
        }

        public string DatabaseBackupPath
        {
            get { return _databaseBackupPath; }
            set { _databaseBackupPath = value; }
        }

        public int EventlogsExpirationDays
        {
            get { return _eventlogsExpirationDays; }
            set { _eventlogsExpirationDays = value; }
        }

        public int EventlogsMaxCountValue
        {
            get { return _eventlogsMaxCountValue; }
            set { _eventlogsMaxCountValue = value; }
        }

        public int EventlogsMaxCountExponent
        {
            get { return _eventlogsMaxCountExponent; }
            set { _eventlogsMaxCountExponent = value; }
        }

        public string EventlogTimeZoneGuidString
        {
            get { return _eventlogTimeZoneGuidString; }
            set { _eventlogTimeZoneGuidString = value; }
        }

        public Color DragDropColorText
        {
            get { return _dragDropColorText; }
            set { _dragDropColorText = value; }
        }
        public Color DragDropColorBackground
        {
            get { return _dragDropColorBackground; }
            set { _dragDropColorBackground = value; }
        }

        public Color ReferenceObjectColorBackground
        {
            get { return _referenceObjectColorBackground; }
            set { _referenceObjectColorBackground = value; }
        }

        public Color ReferenceObjectColorText
        {
            get { return _referenceObjectColorText; }
            set { _referenceObjectColorText = value; }
        }
        public Color AlarmNotAcknowledgedColorText
        {
            get { return _alarmNotAcknowledgedColorText; }
            set { _alarmNotAcknowledgedColorText = value; }
        }
        public Color AlarmNotAcknowledgedColorBackground
        {
            get { return _alarmNotAcknowledgedColorBackground; }
            set { _alarmNotAcknowledgedColorBackground = value; }
        }
        public Color AlarmColorText
        {
            get { return _alarmColorText; }
            set { _alarmColorText = value; }
        }
        public Color AlarmColorBackground
        {
            get { return _alarmColorBackground; }
            set { _alarmColorBackground = value; }
        }
        public Color NormalNotAcknowledgedColorText
        {
            get { return _normalNotAcknowledgedColorText; }
            set { _normalNotAcknowledgedColorText = value; }
        }
        public Color NormalNotAcknowledgedColorBackground
        {
            get { return _normalNotAcknowledgedColorBackground; }
            set { _normalNotAcknowledgedColorBackground = value; }
        }
        public Color NormalColorText
        {
            get { return _normalColorText; }
            set { _normalColorText = value; }
        }
        public Color NormalColorBackground
        {
            get { return _normalColorBackground; }
            set { _normalColorBackground = value; }
        }
        public Color NoAlarmsInQueueColorText
        {
            get { return _noAlarmsInQueueColorText; }
            set { _noAlarmsInQueueColorText = value; }
        }
        public Color NoAlarmsInQueueColorBackground
        {
            get { return _noAlarmsInQueueColorBackground; }
            set { _noAlarmsInQueueColorBackground = value; }
        }

        public bool IsTurnedOn
        {
            get { return _isTurnedOn; }
            set { _isTurnedOn = value; }
        }
        public int AutoCloseTimeout
        {
            get { return _autoCloseTimeout; }
            set { _autoCloseTimeout = value; }
        }

        public int ChangePassDays
        {
            get { return _changePassDays; }
            set { _changePassDays = value; }
        }

        public bool? RequirePINCardLogin
        {
            get { return _requirePINCardLogin; }
            set { _requirePINCardLogin = value; }
        }

        public bool UniqueAndNotNullPersonalKey
        {
            get
            {
                return _uniqueAndNotNullPersonalKey
                    || CgpServer.Singleton.TimetecCommunicationIsEnabled;
            }
            set
            {
                _uniqueAndNotNullPersonalKey = value
                    || CgpServer.Singleton.TimetecCommunicationIsEnabled;
            }
        }

        public bool LockClientApplication
        {
            get { return _lockClientApplication; }
            set { _lockClientApplication = value; }
        }

        public bool CcuConfigurationToServerByPassword
        {
            get { return _ccuConfigurationToServerByPassword; }
            set { _ccuConfigurationToServerByPassword = value; }
        }

        public bool ReqiredSecurePin
        {
            get { return _requiredSecurePin; }
            set { _requiredSecurePin = value; }
        }

        public bool DisableCcuPnPAutomaticAssignmnet
        {
            get { return _disableCcuPnPAutomaticAssignmnet; }
            set { _disableCcuPnPAutomaticAssignmnet = value; }
        }

        public bool ListOnlyUnassignedCardsInPersonForm
        {
            get { return _listOnlyUnassignedCardsInPersonForm; }
            set { _listOnlyUnassignedCardsInPersonForm = value; }
        }

        public int DelayToSaveAlarmsFromCardReaders
        {
            get { return _delayToSaveAlarmsFromCardReaders; }
            set { _delayToSaveAlarmsFromCardReaders = value; }
        }

        public bool UniqueAKeyCSRestriction
        {
            get { return _uniqueAKeyCSRestriction; }
            set { _uniqueAKeyCSRestriction = value; }
        }

        public bool CardReadersAllowPINCachingInMenu
        {
            get { return _cardReadersAllowPINCachingInMenu; }
            set { _cardReadersAllowPINCachingInMenu = value; }
        }

        public int MinimalCodeLength
        {
            get { return _minimalCodeLength; }
            set { _minimalCodeLength = value; }
        }

        public int MaximalCodeLength
        {
            get { return _maximalCodeLength; }
            set { _maximalCodeLength = value; }
        }

        public bool IsPinConfirmationObligatory
        {
            get { return _isPinConfirmationObligatory; }
            set { _isPinConfirmationObligatory = value; }
        }

        public bool EventlogInputStateChanged
        {
            get { return _eventlogInputStateChanged; }
            set { _eventlogInputStateChanged = value; }
        }

        public bool EventlogOutputStateChanged
        {
            get { return _eventlogOutputStateChanged; }
            set { _eventlogOutputStateChanged = value; }
        }

        public bool EventlogAlarmAreaAlarmStateChanged
        {
            get { return _eventlogAlarmAreaAlarmStateChanged; }
            set { _eventlogAlarmAreaAlarmStateChanged = value; }
        }

        public bool EventlogAlarmAreaActivationStateChanged
        {
            get { return _eventlogAlarmAreaActivationStateChanged; }
            set { _eventlogAlarmAreaActivationStateChanged = value; }
        }

        public bool EventlogCardReaderOnlineStateChanged
        {
            get { return _eventlogCardReaderOnlineStateChanged; }
            set { _eventlogCardReaderOnlineStateChanged = value; }
        }

        public bool EventSourcesReverseOrder
        {
            get { return _eventSourcesReverseOrder; }
            set { _eventSourcesReverseOrder = value; }
        }

        public string EventlogReportsTimeZoneGuidString
        {
            get { return _eventlogReportsTimeZoneGuidString; }
            set { _eventlogReportsTimeZoneGuidString = value; }
        }

        public string EventlogReportsEmails
        {
            get { return _eventlogReportsEmails; }
            set { _eventlogReportsEmails = value; }
        }

        public bool EnableLoggingSDPSTZChanges
        {
            get { return _enableLoggingSDPSTZChanges; }
            set { _enableLoggingSDPSTZChanges = value; }
        }

        public bool SyncingTimeFromServer
        {
            get { return _syncingTimeFromServer; }
            set { _syncingTimeFromServer = value; }
        }

        public int PeriodOfTimeSyncWithoutStratum
        {
            get { return _periodOfTimeSyncWithoutStratum; }
            set { _periodOfTimeSyncWithoutStratum = value; }
        }

        public int PeriodicTimeSyncTolerance
        {
            get { return _periodicTimeSyncTolerance; }
            set { _periodicTimeSyncTolerance = value; }
        }

        public int MaxEventsCountForInsert
        {
            get { return _maxEventsCountForInsert; }
            set { _maxEventsCountForInsert = value; }
        }

        public int DelayForSaveEvents
        {
            get { return _delayForSaveEvents; }
            set { _delayForSaveEvents = value; }
        }

        public int ClientSessionTimeOut
        {
            get { return _clientSessionTimeOut; }
            set { _clientSessionTimeOut = value; }
        }

        public int AlarmListSuspendedRefreshTimeout
        {
            get { return _alarmListSuspendedRefreshTimeout; }
            set { _alarmListSuspendedRefreshTimeout = value; }
        }

        public bool CorrectDeserializationFailures
        {
            get { return _correctDeserializationFailures; }
            set { _correctDeserializationFailures = value; }
        }

        public int DelayForSendingChangesToCcu
        {
            get { return _delayForSendingChangesToCcu; }
            set { _delayForSendingChangesToCcu = value; }
        }

        public bool AlarmAreaRestrictivePolicyForTimeBuying
        {
            get { return _alarmAreaRestrictivePolicyForTimeBuying; }
            set { _alarmAreaRestrictivePolicyForTimeBuying = value; }
        }

        #endregion

        /// <summary>
        /// Load settings form registry
        /// </summary>
        private void LoadFromRegistry()
        {
            RegistryKey registryKey = null;

            try
            {
                registryKey = RegistryHelper.GetOrAddKey(CgpServerGlobals.REGISTRY_GENERAL_SETTINGS, false);
                if (null == registryKey)
                {
                    _registryAccessException = new AccessViolationException("Unable to access the registry \"" + CgpServerGlobals.REGISTRY_GENERAL_SETTINGS + "\"");
                    _registryAccessible = false;
                }
            }
            catch
            {
                _registryAccessException = new AccessViolationException("Unable to access the registry \"" + CgpServerGlobals.REGISTRY_GENERAL_SETTINGS + "\"");
                _registryAccessible = false;
            }

            _isConfigured = true;

            try
            {
                _remotingServerPort = (int)registryKey.GetValue(CgpServerGlobals.CGP_REMOTING_SERVER_PORT);
            }
            catch
            {
                _remotingServerPort = CgpServerGlobals.DEFAULT_REMOTING_SERVER_PORT;

                _isConfigured = false;
            }

            try
            {
                _currentLanguage = (string)registryKey.GetValue(CgpServerGlobals.CGP_SERVER_CURRENT_LANGUAGE);
                if (_currentLanguage == null)
                    _currentLanguage = CgpServerGlobals.DEFAULT_LANGUAGE;
            }
            catch
            {
                _currentLanguage = CgpServerGlobals.DEFAULT_LANGUAGE;
            }

            try
            {
                _remotingServerIpAddress = (string)registryKey.GetValue(CgpServerGlobals.CGP_REMOTING_SERVER_IP_ADDRESS);
            }
            catch
            {
                _remotingServerIpAddress = string.Empty;
            }

            try
            {
                _friendlyName = (string)registryKey.GetValue(CgpServerGlobals.CGP_SERVER_FRIENDLY_NAME);
            }
            catch
            {
                _friendlyName = string.Empty;
            }

            try
            {
                _licencePath = (string)registryKey.GetValue(CgpServerGlobals.CGP_SERVER_LICENCE_PATH);
            }
            catch
            {
                _licencePath = string.Empty;
            }

            try
            {
                _showHiddenFeatures = (string[])registryKey.GetValue(CgpServerGlobals.CGP_SHOW_HIDDEN_FEATURES);
            }
            catch
            {
                _showHiddenFeatures = null;
            }
        }

        /// <summary>
        /// Load settings from database
        /// </summary>
        /// 
        private void TryLoadFromDatabase()
        {
            if (!CgpServer.Singleton.IsConnectedToDatabase)
                return;

            if (_isLoadedFromDatabase)
                return;

            lock (_syncRootLoadFromDatabase)
            {
                if (_isLoadedFromDatabase)
                    return;

                LoadFromDatabaseInternal();

                _isLoadedFromDatabase = true;

                if (LoadedFromDatabaseCallbacks == null)
                    return;

                LoadedFromDatabaseCallbacks();
                LoadedFromDatabaseCallbacks = null;
            }
        }

        private DVoid2Void LoadedFromDatabaseCallbacks;

        public void SetLoadedFromDatabaseCallback(DVoid2Void callback)
        {
            lock (_syncRootLoadFromDatabase)
            {
                if (_isLoadedFromDatabase)
                {
                    callback();
                    return;
                }

                LoadedFromDatabaseCallbacks += callback;
            }
        }

        private void LoadFromDatabaseInternal()
        {
            try
            {
                if (DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.AutoStartDHCP,
                        out _autoStartDHCP))
                    CgpServer.Singleton.AutoStartDHCP = _autoStartDHCP;
                else
                    _autoStartDHCP = false;
            }
            catch
            {
                _autoStartDHCP = false;
            }

            //Customer and Supplier info
            try
            {
                string customerCityState = string.Empty;
                if (!DatabaseGeneralOptions.Singleton.Get(
                    DatabaseGeneralOptionType.CustomerCityState,
                    out customerCityState))
                    CustomerCityState = string.Empty;
                else
                    CustomerCityState = customerCityState;
            }
            catch
            {
                CustomerCityState = string.Empty;
            }

            try
            {
                string customerCompanyName = string.Empty;
                if (!DatabaseGeneralOptions.Singleton.Get(
                    DatabaseGeneralOptionType.CustomerCompanyName,
                    out customerCompanyName))
                    CustomerCompanyName = string.Empty;
                else
                    CustomerCompanyName = customerCompanyName;
            }
            catch
            {
                CustomerCompanyName = string.Empty;
            }

            try
            {
                string customerContactPerson = string.Empty;
                if (!DatabaseGeneralOptions.Singleton.Get(
                    DatabaseGeneralOptionType.CustomerContactPerson,
                    out customerContactPerson))
                    CustomerContactPerson = string.Empty;
                else
                    CustomerContactPerson = customerContactPerson;
            }
            catch
            {
                CustomerContactPerson = string.Empty;
            }

            try
            {
                string customerCountry = string.Empty;
                if (!DatabaseGeneralOptions.Singleton.Get(
                    DatabaseGeneralOptionType.CustomerCountry,
                    out customerCountry))
                    CustomerCountry = string.Empty;
                else
                    CustomerCountry = customerCountry;
            }
            catch
            {
                CustomerCountry = string.Empty;
            }

            try
            {
                string customerDeliveryAddress = string.Empty;
                if (!DatabaseGeneralOptions.Singleton.Get(
                    DatabaseGeneralOptionType.CustomerDeliveryAddress,
                    out customerDeliveryAddress))
                    CustomerDeliveryAddress = string.Empty;
                else
                    CustomerDeliveryAddress = customerDeliveryAddress;
            }
            catch
            {
                CustomerDeliveryAddress = string.Empty;
            }

            try
            {
                string customerPhone = string.Empty;
                if (!DatabaseGeneralOptions.Singleton.Get(
                    DatabaseGeneralOptionType.CustomerPhone,
                    out customerPhone))
                    CustomerPhone = string.Empty;
                else
                    CustomerPhone = customerPhone;
            }
            catch
            {
                CustomerPhone = string.Empty;
            }

            try
            {
                string customerWebsite = string.Empty;
                if (!DatabaseGeneralOptions.Singleton.Get(
                    DatabaseGeneralOptionType.CustomerWebsite,
                    out customerWebsite))
                    CustomerWebsite = string.Empty;
                else
                    CustomerWebsite = customerWebsite;
            }
            catch
            {
                CustomerWebsite = string.Empty;
            }

            try
            {
                string customerZipCode = string.Empty;
                if (!DatabaseGeneralOptions.Singleton.Get(
                    DatabaseGeneralOptionType.CustomerZipCode,
                    out customerZipCode))
                    CustomerZipCode = string.Empty;
                else
                    CustomerZipCode = customerZipCode;
            }
            catch
            {
                CustomerZipCode = string.Empty;
            }

            try
            {
                string supplierCityState = string.Empty;
                if (!DatabaseGeneralOptions.Singleton.Get(
                    DatabaseGeneralOptionType.SupplierCityState,
                    out supplierCityState))
                    SupplierCityState = string.Empty;
                else
                    SupplierCityState = supplierCityState;
            }
            catch
            {
                SupplierCityState = string.Empty;
            }

            try
            {
                string supplierCompanyName = string.Empty;
                if (!DatabaseGeneralOptions.Singleton.Get(
                    DatabaseGeneralOptionType.SupplierCompanyName,
                    out supplierCompanyName))
                    SupplierCompanyName = string.Empty;
                else
                    SupplierCompanyName = supplierCompanyName;
            }
            catch
            {
                SupplierCompanyName = string.Empty;
            }

            try
            {
                string supplierContactPerson = string.Empty;
                if (!DatabaseGeneralOptions.Singleton.Get(
                    DatabaseGeneralOptionType.SupplierContactPerson,
                    out supplierContactPerson))
                    SupplierContactPerson = string.Empty;
                else
                    SupplierContactPerson = supplierContactPerson;
            }
            catch
            {
                SupplierContactPerson = string.Empty;
            }

            try
            {
                string supplierCountry = string.Empty;
                if (!DatabaseGeneralOptions.Singleton.Get(
                    DatabaseGeneralOptionType.SupplierCountry,
                    out supplierCountry))
                    SupplierCountry = string.Empty;
                else
                    SupplierCountry = supplierCountry;
            }
            catch
            {
                SupplierCountry = string.Empty;
            }

            try
            {
                string supplierDeliveryAddress = string.Empty;
                if (!DatabaseGeneralOptions.Singleton.Get(
                    DatabaseGeneralOptionType.SupplierDeliveryAddress,
                    out supplierDeliveryAddress))
                    SupplierDeliveryAddress = string.Empty;
                else
                    SupplierDeliveryAddress = supplierDeliveryAddress;
            }
            catch
            {
                SupplierDeliveryAddress = string.Empty;
            }

            try
            {
                string supplierPhone = string.Empty;
                if (!DatabaseGeneralOptions.Singleton.Get(
                    DatabaseGeneralOptionType.SupplierPhone,
                    out supplierPhone))
                    SupplierPhone = string.Empty;
                else
                    SupplierPhone = supplierPhone;
            }
            catch
            {
                SupplierPhone = string.Empty;
            }

            try
            {
                string supplierWebsite = string.Empty;
                if (!DatabaseGeneralOptions.Singleton.Get(
                    DatabaseGeneralOptionType.SupplierWebsite,
                    out supplierWebsite))
                    SupplierWebsite = string.Empty;
                else
                    SupplierWebsite = supplierWebsite;
            }
            catch
            {
                SupplierWebsite = string.Empty;
            }

            try
            {
                string supplierZipCode = string.Empty;
                if (!DatabaseGeneralOptions.Singleton.Get(
                    DatabaseGeneralOptionType.SupplierZipCode,
                    out supplierZipCode))
                    SupplierZipCode = string.Empty;
                else
                    SupplierZipCode = supplierZipCode;
            }
            catch
            {
                SupplierZipCode = string.Empty;
            }

            //SMTP Server Settings
            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.SmtpServer,
                        out _smtpServer))
                    _smtpServer = string.Empty;
            }
            catch
            {
                _smtpServer = string.Empty;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.SmtpPort,
                        out _smtpPort))
                    _smtpPort = CgpServerGlobals.DEFAULT_SMTP_PORT;
            }
            catch
            {
                _smtpPort = CgpServerGlobals.DEFAULT_SMTP_PORT;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.SmtpSourceEmailAddress,
                        out _smtpSourceEmailAddress))
                    _smtpSourceEmailAddress = string.Empty;
            }
            catch
            {
                _smtpSourceEmailAddress = string.Empty;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.SmtpSubject,
                        out _smtpSubject))
                    _smtpSubject = string.Empty;
            }
            catch
            {
                _smtpSubject = string.Empty;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.SmtpCredentials,
                        out _smtpCredentials))
                    _smtpCredentials = string.Empty;
            }
            catch
            {
                _smtpCredentials = string.Empty;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.SmtpSsl,
                        out _smtpSsl))
                    _smtpSsl = false;
            }
            catch
            {
                _smtpSsl = false;
            }

            //Serial Port Settings 
            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.SerialPort,
                        out _serialPort))
                    _serialPort = string.Empty;
            }
            catch
            {
                _serialPort = string.Empty;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.SerialPortBaudRate,
                        out _serialPortBaudRate))
                    _serialPortBaudRate = 9600;
            }
            catch
            {
                _serialPortBaudRate = 9600;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.SerialPortDataBits,
                        out _serialPortDataBits))
                    _serialPortDataBits = 8;
            }
            catch
            {
                _serialPortDataBits = 8;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.SerialPortParity,
                        out _serailPortParity) ||
                    _serailPortParity == null)
                    _serailPortParity = "N";
            }
            catch
            {
                _serailPortParity = "N";
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.SerialPortStopBits,
                        out _serialPortStopBits) ||
                    _serialPortStopBits == null)
                    _serialPortStopBits = "1";
            }
            catch
            {
                _serialPortStopBits = "1";
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.SerialPortFlowControl,
                        out _serialPortFlowControl))
                    _serialPortFlowControl = string.Empty;
            }
            catch
            {
                _serialPortFlowControl = string.Empty;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.SerialPortParityCheck,
                        out _serialPortParityCheck))
                    _serialPortParityCheck = false;
            }
            catch
            {
                _serialPortParityCheck = false;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.SerialPortCarrierDetect,
                        out _serialPortCarrierDetect))
                    _serialPortCarrierDetect = false;
            }
            catch
            {
                _serialPortCarrierDetect = false;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.SerialPortPin,
                        out _serialPortPin))
                    _serialPortPin = null;
            }
            catch
            {
                _serialPortPin = null;
            }

            //database backup settings
            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.DatabaseBackupTimeZone,
                        out _timeZoneGuidString))
                    _timeZoneGuidString = string.Empty;
            }
            catch
            {
                _timeZoneGuidString = string.Empty;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.DatabaseBackupPath,
                        out _databaseBackupPath))
                    _databaseBackupPath = string.Empty;
            }
            catch
            {
                _databaseBackupPath = string.Empty;
            }

            //eventlog expiration
            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.EventlogsExpirationDays,
                        out _eventlogsExpirationDays))
                    _eventlogsExpirationDays = 90;

                if (_eventlogsExpirationDays == 0)
                    _eventlogsExpirationDays = 90;
            }
            catch
            {
                _eventlogsExpirationDays = 90;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.EventlogsMaxCountValue,
                        out _eventlogsMaxCountValue))
                    _eventlogsMaxCountValue = 100;

                if (_eventlogsMaxCountValue == 0)
                    _eventlogsMaxCountValue = 100;
            }
            catch
            {
                _eventlogsMaxCountValue = 100;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.EventlogsMaxCountExponent,
                        out _eventlogsMaxCountExponent))
                    _eventlogsMaxCountExponent = 1;

                if (_eventlogsMaxCountExponent == 0)
                    _eventlogsMaxCountExponent = 1;
            }
            catch
            {
                _eventlogsMaxCountExponent = 1;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.EventlogsExpirationTimeZone,
                        out _eventlogTimeZoneGuidString))
                    _eventlogTimeZoneGuidString = string.Empty;
            }
            catch
            {
                _eventlogTimeZoneGuidString = string.Empty;
            }

            //color settings
            try
            {
                int colorFromDatabase;

                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpDragDropColorBackground,
                        out colorFromDatabase))
                    _dragDropColorBackground =
                        CgpServerGlobals.DRAG_DROP_COLOR_BACKGROUND;
                else
                {
                    _dragDropColorBackground =
                        Color.FromArgb(colorFromDatabase);

                    if (_dragDropColorBackground.IsEmpty)
                        _dragDropColorBackground =
                            CgpServerGlobals.DRAG_DROP_COLOR_BACKGROUND;
                }
            }
            catch
            {
                _dragDropColorBackground =
                    CgpServerGlobals.DRAG_DROP_COLOR_BACKGROUND;
            }

            try
            {
                int colorFromDatabase;

                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpDragDropColorText,
                        out colorFromDatabase))
                    _dragDropColorText =
                        CgpServerGlobals.DRAG_DROP_COLOR_TEXT;
                else
                {
                    _dragDropColorText =
                        Color.FromArgb(colorFromDatabase);

                    if (_dragDropColorText.IsEmpty)
                        _dragDropColorText =
                            CgpServerGlobals.DRAG_DROP_COLOR_TEXT;
                }
            }
            catch
            {
                _dragDropColorText =
                    CgpServerGlobals.DRAG_DROP_COLOR_TEXT;
            }

            try
            {
                int colorFromDatabase;

                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpReferenceObjectColorBackground,
                        out colorFromDatabase))
                    _referenceObjectColorBackground =
                        CgpServerGlobals.REFERENCE_OBJECT_COLOR_BACKGROUND;
                else
                {
                    _referenceObjectColorBackground =
                        Color.FromArgb(colorFromDatabase);

                    if (_referenceObjectColorBackground.IsEmpty)
                        _referenceObjectColorBackground =
                            CgpServerGlobals.REFERENCE_OBJECT_COLOR_BACKGROUND;
                }
            }
            catch
            {
                _referenceObjectColorBackground =
                    CgpServerGlobals.REFERENCE_OBJECT_COLOR_BACKGROUND;
            }

            try
            {
                int colorFromDatabase;

                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpReferenceObjectColorText,
                        out colorFromDatabase))
                    _referenceObjectColorText =
                        CgpServerGlobals.REFERENCE_OBJECT_COLOR_TEXT;
                else
                {
                    _referenceObjectColorText =
                        Color.FromArgb(colorFromDatabase);

                    if (_referenceObjectColorText.IsEmpty)
                        _referenceObjectColorText =
                            CgpServerGlobals.REFERENCE_OBJECT_COLOR_TEXT;
                }
            }
            catch
            {
                _referenceObjectColorText =
                    CgpServerGlobals.REFERENCE_OBJECT_COLOR_TEXT;
            }

            try
            {
                int colorFromDatabase;

                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpAlarmNotAcknowledgedColorText,
                        out colorFromDatabase))
                    _alarmNotAcknowledgedColorText =
                        CgpServerGlobals.ALARM_NOT_ACKNOWLEDGED_COLOR_TEXT;
                else
                {
                    _alarmNotAcknowledgedColorText =
                        Color.FromArgb(colorFromDatabase);

                    if (_alarmNotAcknowledgedColorText.IsEmpty)
                        _alarmNotAcknowledgedColorText =
                            CgpServerGlobals.ALARM_NOT_ACKNOWLEDGED_COLOR_TEXT;
                }
            }
            catch
            {
                _alarmNotAcknowledgedColorText =
                    CgpServerGlobals.ALARM_NOT_ACKNOWLEDGED_COLOR_TEXT;
            }

            try
            {
                int colorFromDatabase;

                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpAlarmNotAcknowledgedColorBackground,
                        out colorFromDatabase))
                    _alarmNotAcknowledgedColorBackground =
                        CgpServerGlobals.ALARM_NOT_ACKNOWLEDGED_COLOR_BACKGROUND;
                else
                {
                    _alarmNotAcknowledgedColorBackground =
                        Color.FromArgb(colorFromDatabase);

                    if (_alarmNotAcknowledgedColorBackground.IsEmpty)
                        _alarmNotAcknowledgedColorBackground =
                            CgpServerGlobals.ALARM_NOT_ACKNOWLEDGED_COLOR_BACKGROUND;
                }
            }
            catch
            {
                _alarmNotAcknowledgedColorBackground =
                    CgpServerGlobals.ALARM_NOT_ACKNOWLEDGED_COLOR_BACKGROUND;
            }

            try
            {
                int colorFromDatabase;

                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpAlarmColorText,
                        out colorFromDatabase))
                    _alarmColorText =
                        CgpServerGlobals.ALARM_COLOR_TEXT;
                else
                {
                    _alarmColorText =
                        Color.FromArgb(colorFromDatabase);

                    if (_alarmColorText.IsEmpty)
                        _alarmColorText =
                            CgpServerGlobals.ALARM_COLOR_TEXT;
                }
            }
            catch
            {
                _alarmColorText =
                    CgpServerGlobals.ALARM_COLOR_TEXT;
            }

            try
            {
                int colorFromDatabase;

                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpAlarmColorBackground,
                        out colorFromDatabase))
                    _alarmColorBackground =
                        CgpServerGlobals.ALARM_COLOR_BACKGROUND;
                else
                {
                    _alarmColorBackground =
                        Color.FromArgb(colorFromDatabase);

                    if (_alarmColorBackground.IsEmpty)
                        _alarmColorBackground =
                            CgpServerGlobals.ALARM_COLOR_BACKGROUND;
                }
            }
            catch
            {
                _alarmColorBackground =
                    CgpServerGlobals.ALARM_COLOR_BACKGROUND;
            }

            try
            {
                int colorFromDatabase;

                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpNormalNotAcknowledgedColorText,
                        out colorFromDatabase))
                    _normalNotAcknowledgedColorText =
                        CgpServerGlobals.NORMAL_NOT_ACKNOWLEDGED_COLOR_TEXT;
                else
                {
                    _normalNotAcknowledgedColorText =
                        Color.FromArgb(colorFromDatabase);

                    if (_normalNotAcknowledgedColorText.IsEmpty)
                        _normalNotAcknowledgedColorText =
                            CgpServerGlobals.NORMAL_NOT_ACKNOWLEDGED_COLOR_TEXT;
                }
            }
            catch
            {
                _normalNotAcknowledgedColorText =
                    CgpServerGlobals.NORMAL_NOT_ACKNOWLEDGED_COLOR_TEXT;
            }

            try
            {
                int colorFromDatabase;

                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpNormalNotAcknowledgedColorBackground,
                        out colorFromDatabase))
                    _normalNotAcknowledgedColorBackground =
                        CgpServerGlobals.NORMAL_NOT_ACKNOWLEDGED_COLOR_BACKGROUND;
                else
                {
                    _normalNotAcknowledgedColorBackground =
                        Color.FromArgb(colorFromDatabase);

                    if (_normalNotAcknowledgedColorBackground.IsEmpty)
                        _normalNotAcknowledgedColorBackground =
                            CgpServerGlobals.NORMAL_NOT_ACKNOWLEDGED_COLOR_BACKGROUND;
                }
            }
            catch
            {
                _normalNotAcknowledgedColorBackground =
                    CgpServerGlobals.NORMAL_NOT_ACKNOWLEDGED_COLOR_BACKGROUND;
            }

            try
            {
                int colorFromDatabase;

                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpNormalColorText,
                        out colorFromDatabase))
                    _normalColorText =
                        CgpServerGlobals.NORMAL_COLOR_TEXT;
                else
                {
                    _normalColorText =
                        Color.FromArgb(colorFromDatabase);

                    if (_normalColorText.IsEmpty)
                        _normalColorText =
                            CgpServerGlobals.NORMAL_COLOR_TEXT;
                }
            }
            catch
            {
                _normalColorText =
                    CgpServerGlobals.NORMAL_COLOR_TEXT;
            }

            try
            {
                int colorFromDatabase;

                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpNormalColorBackground,
                        out colorFromDatabase))
                    _normalColorBackground =
                        CgpServerGlobals.NORMAL_COLOR_BACKGROUND;
                else
                {
                    _normalColorBackground =
                        Color.FromArgb(colorFromDatabase);

                    if (_normalColorBackground.IsEmpty)
                        _normalColorBackground =
                            CgpServerGlobals.NORMAL_COLOR_BACKGROUND;
                }
            }
            catch
            {
                _normalColorBackground =
                    CgpServerGlobals.NORMAL_COLOR_BACKGROUND;
            }

            try
            {
                int colorFromDatabase;

                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpNoAlarmsInQueueColorText,
                        out colorFromDatabase))
                    _noAlarmsInQueueColorText =
                        CgpServerGlobals.NO_ALARMS_IN_QUEUE_COLOR_TEXT;
                else
                {
                    _noAlarmsInQueueColorText =
                        Color.FromArgb(colorFromDatabase);

                    if (_noAlarmsInQueueColorText.IsEmpty)
                        _noAlarmsInQueueColorText =
                            CgpServerGlobals.NO_ALARMS_IN_QUEUE_COLOR_TEXT;
                }
            }
            catch
            {
                _noAlarmsInQueueColorText =
                    CgpServerGlobals.NO_ALARMS_IN_QUEUE_COLOR_TEXT;
            }

            try
            {
                int colorFromDatabase;

                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpNoAlarmsInQueueColorBackground,
                        out colorFromDatabase))
                    _noAlarmsInQueueColorBackground =
                        CgpServerGlobals.NO_ALARMS_IN_QUEUE_COLOR_BACKGROUND;
                else
                {
                    _noAlarmsInQueueColorBackground =
                        Color.FromArgb(colorFromDatabase);

                    if (_noAlarmsInQueueColorBackground.IsEmpty)
                        _noAlarmsInQueueColorBackground =
                            CgpServerGlobals.NO_ALARMS_IN_QUEUE_COLOR_BACKGROUND;
                }
            }
            catch
            {
                _noAlarmsInQueueColorBackground =
                    CgpServerGlobals.NO_ALARMS_IN_QUEUE_COLOR_BACKGROUND;
            }

            //AutoClose
            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpAutoCloseTurnedOn,
                        out _isTurnedOn))
                    _isTurnedOn = false;
            }
            catch
            {
                _isTurnedOn = false;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpAutoCloseTimeout,
                        out _autoCloseTimeout))
                    _autoCloseTimeout = 60;
            }
            catch
            {
                _autoCloseTimeout = 60;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpChangePasswordDay,
                        out _changePassDays))
                    _changePassDays = 0;
            }
            catch
            {
                _changePassDays = 0;
            }

            //security settings
            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpRequirePINCardLogin,
                        out _requirePINCardLogin))
                    _requirePINCardLogin = false;
            }
            catch
            {
                _requirePINCardLogin = false;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpUniqueNotNullPersonalKey,
                        out _uniqueAndNotNullPersonalKey))
                    _uniqueAndNotNullPersonalKey = false;
            }
            catch
            {
                _uniqueAndNotNullPersonalKey = false;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpLockClientApplication,
                        out _lockClientApplication))
                    _lockClientApplication = false;
            }
            catch
            {
                _lockClientApplication = false;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CcuConfigurationToServerByPassword,
                        out _ccuConfigurationToServerByPassword))
                    _ccuConfigurationToServerByPassword = true;
            }
            catch
            {
                _ccuConfigurationToServerByPassword = true;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CgpRequredSecurePin,
                        out _requiredSecurePin))
                    _requiredSecurePin = false;
            }
            catch
            {
                _requiredSecurePin = false;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.DisableCcuPnPAssigmnet,
                        out _disableCcuPnPAutomaticAssignmnet))
                    _disableCcuPnPAutomaticAssignmnet = false;
            }
            catch
            {
                _disableCcuPnPAutomaticAssignmnet = false;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.ListOnlyUnassignedCardsInPersonForm,
                        out _listOnlyUnassignedCardsInPersonForm))
                    _listOnlyUnassignedCardsInPersonForm = false;
            }
            catch
            {
                _listOnlyUnassignedCardsInPersonForm = false;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.DelayToSaveAlarmsFromCardReaders,
                        out _delayToSaveAlarmsFromCardReaders))
                    _delayToSaveAlarmsFromCardReaders =
                        CgpServerGlobals.DEFAULT_DELAY_TO_SAVE_ALARMS_FROM_CARD_READERS;
            }
            catch
            {
                _delayToSaveAlarmsFromCardReaders =
                    CgpServerGlobals.DEFAULT_DELAY_TO_SAVE_ALARMS_FROM_CARD_READERS;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.UniqueAKeyCSRestriction,
                        out _uniqueAKeyCSRestriction))
                    _uniqueAKeyCSRestriction = true;
            }
            catch
            {
                _uniqueAKeyCSRestriction = true;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.CardReadersAllowPINCachingInMenu,
                        out _cardReadersAllowPINCachingInMenu))
                    _cardReadersAllowPINCachingInMenu = false;
            }
            catch
            {
                _cardReadersAllowPINCachingInMenu = false;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.MinimalCodeLength,
                        out _minimalCodeLength))
                    _minimalCodeLength = 4;
            }
            catch
            {
                _minimalCodeLength = 4;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.MaximalCodeLength,
                        out _maximalCodeLength))
                    _maximalCodeLength = 12;
            }
            catch
            {
                _maximalCodeLength = 12;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.IsPinConfirmationObligatory,
                        out _isPinConfirmationObligatory))
                {
                    _isPinConfirmationObligatory = true;
                }
            }
            catch
            {
                _isPinConfirmationObligatory = true;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.EventlogInputStateChanged,
                        out _eventlogInputStateChanged))
                    _eventlogInputStateChanged = true;
            }
            catch
            {
                _eventlogInputStateChanged = true;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.EventlogOutputStateChanged,
                        out _eventlogOutputStateChanged))
                    _eventlogOutputStateChanged = true;
            }
            catch
            {
                _eventlogOutputStateChanged = true;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.EventlogAlarmAreaAlarmStateChanged,
                        out _eventlogAlarmAreaAlarmStateChanged))
                    _eventlogAlarmAreaAlarmStateChanged = true;
            }
            catch
            {
                _eventlogAlarmAreaAlarmStateChanged = true;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.EventlogAlarmAreaActivationStateChanged,
                        out _eventlogAlarmAreaActivationStateChanged))
                    _eventlogAlarmAreaActivationStateChanged = true;
            }
            catch
            {
                _eventlogAlarmAreaActivationStateChanged = true;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.EventSourcesReverseOrder,
                        out _eventSourcesReverseOrder))
                    _eventSourcesReverseOrder = true;
            }
            catch
            {
                _eventSourcesReverseOrder = true;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.EventlogReportsTimezoneGuidString,
                        out _eventlogReportsTimeZoneGuidString))
                    _eventlogReportsTimeZoneGuidString = string.Empty;
            }
            catch
            {
                _eventlogReportsTimeZoneGuidString = string.Empty;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.EventlogReportsEmails,
                        out _eventlogReportsEmails))
                    _eventlogReportsEmails = string.Empty;
            }
            catch
            {
                _eventlogReportsEmails = string.Empty;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.EventlogCardReaderOnlineStateChanged,
                        out _eventlogCardReaderOnlineStateChanged))
                    _eventlogCardReaderOnlineStateChanged = true;
            }
            catch
            {
                _eventlogCardReaderOnlineStateChanged = true;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.AdvancedDeviceSettingsEnableLoggingSDPSTZChanges,
                        out _enableLoggingSDPSTZChanges))
                    _enableLoggingSDPSTZChanges = false;
            }
            catch
            {
                _enableLoggingSDPSTZChanges = false;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.AdvancedSettingsMaxEventsCountForInsert,
                        out _maxEventsCountForInsert))
                    _maxEventsCountForInsert =
                        CgpServerGlobals.DEFAULT_MAX_EVENTS_COUNT_FOR_INSERT;
            }
            catch
            {
                _maxEventsCountForInsert =
                    CgpServerGlobals.DEFAULT_MAX_EVENTS_COUNT_FOR_INSERT;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.AdvancedSettingsDelayForSaveEvents,
                        out _delayForSaveEvents))
                    _delayForSaveEvents =
                        CgpServerGlobals.DEFAULT_DELAY_FOR_SAVE_EVENTS;
            }
            catch
            {
                _delayForSaveEvents =
                    CgpServerGlobals.DEFAULT_DELAY_FOR_SAVE_EVENTS;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.AdvancedSettingsClientSessionTimeout,
                        out _clientSessionTimeOut))
                    _clientSessionTimeOut =
                        CgpServerGlobals.DEFAULT_CLIENT_SESION_TIMEOUT;
            }
            catch
            {
                _clientSessionTimeOut =
                    CgpServerGlobals.DEFAULT_CLIENT_SESION_TIMEOUT;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.AlarmListSuspendedRefreshTimeout,
                        out _alarmListSuspendedRefreshTimeout))
                    _alarmListSuspendedRefreshTimeout =
                        CgpServerGlobals.DEFAULT_ALARM_LIST_SUSPEND_REFRESH_TIMEOUT;
            }
            catch
            {
                _alarmListSuspendedRefreshTimeout =
                    CgpServerGlobals.DEFAULT_ALARM_LIST_SUSPEND_REFRESH_TIMEOUT;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.AdvancedSettingsCorrectDeserializationFailures,
                        out _correctDeserializationFailures))
                    _correctDeserializationFailures = true;
            }
            catch
            {
                _correctDeserializationFailures = true;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                    DatabaseGeneralOptionType.AdvancedSettingsDelayForSendingChangesToCcu,
                    out _delayForSendingChangesToCcu))
                    _delayForSendingChangesToCcu =
                        CgpServerGlobals.DEFAULT_DELAY_FOR_SENDING_CHANGES_TO_CCU;
            }
            catch
            {
                _delayForSendingChangesToCcu =
                    CgpServerGlobals.DEFAULT_DELAY_FOR_SENDING_CHANGES_TO_CCU;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.AdvancedDeviceSettingsSyncingTimeFromServer,
                        out _syncingTimeFromServer))
                    _syncingTimeFromServer = false;
            }
            catch
            {
                _syncingTimeFromServer = false;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.AdvancedDeviceSettingsPeriodOfTimeSyncWithoutStratum,
                        out _periodOfTimeSyncWithoutStratum))
                    _periodOfTimeSyncWithoutStratum =
                        CgpServerGlobals.DEFAULT_PERIOD_OF_TIME_SYNC_WITHOUT_STRATUM;
            }
            catch
            {
                _periodOfTimeSyncWithoutStratum =
                    CgpServerGlobals.DEFAULT_PERIOD_OF_TIME_SYNC_WITHOUT_STRATUM;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                        DatabaseGeneralOptionType.AdvancedDeviceSettingsPeriodicTimeSyncTolerance,
                        out _periodicTimeSyncTolerance))
                    _periodicTimeSyncTolerance =
                        CgpServerGlobals.DEFAULT_PERIODIC_TIME_SYNC_TOLERANCE;
            }
            catch
            {
                _periodicTimeSyncTolerance =
                    CgpServerGlobals.DEFAULT_PERIODIC_TIME_SYNC_TOLERANCE;
            }

            try
            {
                if (!DatabaseGeneralOptions.Singleton.Get(
                    DatabaseGeneralOptionType.AlarmAreaRestrictivePolicyForTimeBuying,
                    out _alarmAreaRestrictivePolicyForTimeBuying))
                    _alarmAreaRestrictivePolicyForTimeBuying = true;
            }
            catch
            {
                _alarmAreaRestrictivePolicyForTimeBuying = true;
            }
        }

        /// <summary>
        /// Insert settings to database and remove from registry
        /// </summary>
        public void InsertToDatabaseAndRemoveFromRegistry()
        {
            if (DatabaseGeneralOptions.Singleton.GetSavedValuesFromRegistryToDatabase())
                return;

            RegistryKey registryKey = null;
            try
            {
                registryKey = RegistryHelper.GetOrAddKey(CgpServerGlobals.REGISTRY_GENERAL_SETTINGS, true);
                if (null == registryKey)
                {
                    _registryAccessException = new AccessViolationException("Unable to access the registry \"" + CgpServerGlobals.REGISTRY_GENERAL_SETTINGS + "\"");
                    _registryAccessible = false;
                }
            }
            catch
            {
                _registryAccessException = new AccessViolationException("Unable to access the registry \"" + CgpServerGlobals.REGISTRY_GENERAL_SETTINGS + "\"");
                _registryAccessible = false;
            }

            try
            {
                _autoStartDHCP = bool.Parse(registryKey.GetValue(CgpServerGlobals.CGP_DHCP_AUTOSTART).ToString());
                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.AutoStartDHCP, _autoStartDHCP);
                registryKey.DeleteValue(CgpServerGlobals.CGP_DHCP_AUTOSTART);
            }
            catch { }

            //SMTP Server Settings
            try
            {
                string smtpServerFromRegistry = (string)registryKey.GetValue(CgpServerGlobals.CGP_SMTP_SERVER);
                if (smtpServerFromRegistry != null)
                {
                    _smtpServer = smtpServerFromRegistry;
                    DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SmtpServer, _smtpServer);
                }

                registryKey.DeleteValue(CgpServerGlobals.CGP_SMTP_SERVER);
            }
            catch { }

            try
            {
                _smtpPort = (int)registryKey.GetValue(CgpServerGlobals.CGP_SMTP_PORT);
                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SmtpPort, _smtpPort);
                registryKey.DeleteValue(CgpServerGlobals.CGP_SMTP_PORT);
            }
            catch { }

            try
            {
                string smtpSourceEmailAddressFromRegistry = (string)registryKey.GetValue(CgpServerGlobals.CGP_SMTP_SOURCE_EMAIL_ADDRESS);
                if (smtpSourceEmailAddressFromRegistry != null)
                {
                    _smtpSourceEmailAddress = smtpSourceEmailAddressFromRegistry;
                    DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SmtpSourceEmailAddress, _smtpSourceEmailAddress);
                }

                registryKey.DeleteValue(CgpServerGlobals.CGP_SMTP_SOURCE_EMAIL_ADDRESS);
            }
            catch { }

            try
            {
                string smtpSubjectFromRegistry = (string)registryKey.GetValue(CgpServerGlobals.CGP_SMTP_SUBJECT);
                if (smtpSubjectFromRegistry != null)
                {
                    _smtpSubject = smtpSubjectFromRegistry;
                    DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SmtpSubject, _smtpSubject);
                }

                registryKey.DeleteValue(CgpServerGlobals.CGP_SMTP_SUBJECT);
            }
            catch { }

            try
            {
                string smtpCredentialsFromRegistry = (string)registryKey.GetValue(CgpServerGlobals.CGP_SMTP_CREDENTIALS);
                if (smtpCredentialsFromRegistry != null)
                {
                    _smtpCredentials = smtpCredentialsFromRegistry;
                    DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SmtpCredentials, _smtpCredentials);
                }

                registryKey.DeleteValue(CgpServerGlobals.CGP_SMTP_CREDENTIALS);
            }
            catch { }

            try
            {
                string smtpSslFromRegistry = (string)registryKey.GetValue(CgpServerGlobals.CGP_SMTP_SSL);
                if (smtpSslFromRegistry != null)
                {
                    _smtpSsl = bool.Parse(smtpSslFromRegistry);
                    DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SmtpSsl, _smtpSsl);
                }

                registryKey.DeleteValue(CgpServerGlobals.CGP_SMTP_SSL);
            }
            catch { }

            //Serial Port Settings 
            try
            {
                string serialPortFromRegistry = (string)registryKey.GetValue(CgpServerGlobals.CGP_SERIAL_PORT);
                if (serialPortFromRegistry != null)
                {
                    _serialPort = serialPortFromRegistry;
                    DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SerialPort, _serialPort);
                }

                registryKey.DeleteValue(CgpServerGlobals.CGP_SERIAL_PORT);
            }
            catch { }

            try
            {
                _serialPortBaudRate = (int)registryKey.GetValue(CgpServerGlobals.CGP_SERIAL_PORT_BAUD_RATE);
                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SerialPortBaudRate, _serialPortBaudRate);
                registryKey.DeleteValue(CgpServerGlobals.CGP_SERIAL_PORT_BAUD_RATE);
            }
            catch { }

            try
            {
                _serialPortDataBits = (int)registryKey.GetValue(CgpServerGlobals.CGP_SERIAL_PORT_DATA_BITS);
                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SerialPortDataBits, _serialPortDataBits);
                registryKey.DeleteValue(CgpServerGlobals.CGP_SERIAL_PORT_DATA_BITS);
            }
            catch { }

            try
            {
                string serailPortParityFromRegistry = (string)registryKey.GetValue(CgpServerGlobals.CGP_SERIAL_PORT_PARITY);
                if (serailPortParityFromRegistry != null)
                {
                    _serailPortParity = serailPortParityFromRegistry;
                    DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SerialPortParity, _serailPortParity);
                }

                registryKey.DeleteValue(CgpServerGlobals.CGP_SERIAL_PORT_PARITY);
            }
            catch { }

            try
            {
                string serialPortStopBitsFromRegistry = (string)registryKey.GetValue(CgpServerGlobals.CGP_SERIAL_PORT_STOP_BITS);
                if (serialPortStopBitsFromRegistry != null)
                {
                    _serialPortStopBits = serialPortStopBitsFromRegistry;
                    DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SerialPortStopBits, _serialPortStopBits);
                }

                registryKey.DeleteValue(CgpServerGlobals.CGP_SERIAL_PORT_STOP_BITS);
            }
            catch { }

            try
            {
                string serialPortFlowControlFromRegistry = (string)registryKey.GetValue(CgpServerGlobals.CGP_SERIAL_PORT_FLOW_CONTROL);
                if (serialPortFlowControlFromRegistry != null)
                {
                    _serialPortFlowControl = serialPortFlowControlFromRegistry;
                    DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SerialPortFlowControl, _serialPortFlowControl);
                }

                registryKey.DeleteValue(CgpServerGlobals.CGP_SERIAL_PORT_FLOW_CONTROL);
            }
            catch { }

            try
            {
                _serialPortParityCheck = (int?)registryKey.GetValue(CgpServerGlobals.CGP_SERIAL_PORT_PARITY_CHECK) != 0;
                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SerialPortParityCheck, _serialPortParityCheck);
                registryKey.DeleteValue(CgpServerGlobals.CGP_SERIAL_PORT_PARITY_CHECK);
            }
            catch { }

            try
            {
                _serialPortCarrierDetect = (int?)registryKey.GetValue(CgpServerGlobals.CGP_SERIAL_PORT_CARRIER_DETECT) != 0;
                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SerialPortCarrierDetect, _serialPortCarrierDetect);
                registryKey.DeleteValue(CgpServerGlobals.CGP_SERIAL_PORT_CARRIER_DETECT);
            }
            catch { }

            try
            {
                _serialPortPin = (int?)registryKey.GetValue(CgpServerGlobals.CGP_SERIAL_PORT_PIN);
                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SerialPortPin, _serialPortPin);
                registryKey.DeleteValue(CgpServerGlobals.CGP_SERIAL_PORT_PIN);
            }
            catch { }

            //database backup settings
            try
            {
                string timeZoneGuidStringFromRegistry = (string)registryKey.GetValue(CgpServerGlobals.CGP_DATABASE_BACKUP_TIME_ZONE);
                if (timeZoneGuidStringFromRegistry != null)
                {
                    _timeZoneGuidString = timeZoneGuidStringFromRegistry;
                    DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.DatabaseBackupTimeZone, _timeZoneGuidString);
                }

                registryKey.DeleteValue(CgpServerGlobals.CGP_DATABASE_BACKUP_TIME_ZONE);
            }
            catch { }

            try
            {
                string databaseBackupPathFromRegistry = (string)registryKey.GetValue(CgpServerGlobals.CGP_DATABASE_BACKUP_PATH);
                if (databaseBackupPathFromRegistry != null)
                {
                    _databaseBackupPath = databaseBackupPathFromRegistry;
                    DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.DatabaseBackupPath, _databaseBackupPath);
                }

                registryKey.DeleteValue(CgpServerGlobals.CGP_DATABASE_BACKUP_PATH);
            }
            catch { }

            //eventlog expiration
            try
            {
                _eventlogsExpirationDays = (int)registryKey.GetValue(CgpServerGlobals.CGP_EVENTLOGS_EXPIRATION_DAYS);
                if (_eventlogsExpirationDays == 0)
                {
                    _eventlogsExpirationDays = 90;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogsExpirationDays, _eventlogsExpirationDays);
                registryKey.DeleteValue(CgpServerGlobals.CGP_EVENTLOGS_EXPIRATION_DAYS);
            }
            catch { }

            try
            {
                _eventlogsMaxCountValue = (int)registryKey.GetValue(CgpServerGlobals.CGP_EVENTLOGS_MAX_COUNT_VALUES);
                if (_eventlogsMaxCountValue == 0)
                {
                    _eventlogsMaxCountValue = 100;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogsMaxCountValue, _eventlogsMaxCountValue);
                registryKey.DeleteValue(CgpServerGlobals.CGP_EVENTLOGS_MAX_COUNT_VALUES);
            }
            catch { }

            try
            {
                _eventlogsMaxCountExponent = (int)registryKey.GetValue(CgpServerGlobals.CGP_EVENTLOGS_MAX_COUNT_EXPONENT);
                if (_eventlogsMaxCountExponent == 0)
                {
                    _eventlogsMaxCountExponent = 1;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogsMaxCountExponent, _eventlogsMaxCountExponent);
                registryKey.DeleteValue(CgpServerGlobals.CGP_EVENTLOGS_MAX_COUNT_EXPONENT);
            }
            catch { }

            try
            {
                string eventlogTimeZoneGuidStringFromRegistry = (string)registryKey.GetValue(CgpServerGlobals.CGP_EVENTLOGS_EXPIRATION_TIME_ZONE);
                if (eventlogTimeZoneGuidStringFromRegistry != null)
                {
                    _eventlogTimeZoneGuidString = eventlogTimeZoneGuidStringFromRegistry;
                    DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogsExpirationTimeZone, _eventlogTimeZoneGuidString);
                }

                registryKey.DeleteValue(CgpServerGlobals.CGP_EVENTLOGS_EXPIRATION_TIME_ZONE);
            }
            catch { }

            //color settings
            try
            {
                int colorFromRegistry = (int)registryKey.GetValue(CgpServerGlobals.CGP_DRAG_DROP_COLOR_BACKGROUND);
                _dragDropColorBackground = Color.FromArgb(colorFromRegistry);
                if (_dragDropColorBackground.IsEmpty)
                {
                    _dragDropColorBackground = CgpServerGlobals.DRAG_DROP_COLOR_BACKGROUND;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpDragDropColorBackground, colorFromRegistry);
                registryKey.DeleteValue(CgpServerGlobals.CGP_DRAG_DROP_COLOR_BACKGROUND);
            }
            catch { }

            try
            {
                int colorFromRegistry = (int)registryKey.GetValue(CgpServerGlobals.CGP_DRAG_DROP_COLOR_TEXT);
                _dragDropColorText = Color.FromArgb(colorFromRegistry);
                if (_dragDropColorText.IsEmpty)
                {
                    _dragDropColorText = CgpServerGlobals.DRAG_DROP_COLOR_TEXT;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpDragDropColorText, colorFromRegistry);
                registryKey.DeleteValue(CgpServerGlobals.CGP_DRAG_DROP_COLOR_TEXT);
            }
            catch { }

            try
            {
                int colorFromRegistry = (int)registryKey.GetValue(CgpServerGlobals.CGP_REFERENCE_OBJECT_COLOR_BACKGROUND);
                _referenceObjectColorBackground = Color.FromArgb(colorFromRegistry);
                if (_referenceObjectColorBackground.IsEmpty)
                {
                    _referenceObjectColorBackground = CgpServerGlobals.REFERENCE_OBJECT_COLOR_BACKGROUND;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpReferenceObjectColorBackground, colorFromRegistry);
                registryKey.DeleteValue(CgpServerGlobals.CGP_REFERENCE_OBJECT_COLOR_BACKGROUND);
            }
            catch { }

            try
            {
                int colorFromRegistry = (int)registryKey.GetValue(CgpServerGlobals.CGP_REFERENCE_OBJECT_COLOR_TEXT);
                _referenceObjectColorText = Color.FromArgb(colorFromRegistry);
                if (_referenceObjectColorText.IsEmpty)
                {
                    _referenceObjectColorText = CgpServerGlobals.REFERENCE_OBJECT_COLOR_TEXT;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpReferenceObjectColorText, colorFromRegistry);
                registryKey.DeleteValue(CgpServerGlobals.CGP_REFERENCE_OBJECT_COLOR_TEXT);
            }
            catch { }

            try
            {
                int colorFromRegistry = (int)registryKey.GetValue(CgpServerGlobals.CGP_ALARM_NOT_ACKNOWLEDGED_COLOR_TEXT);
                _alarmNotAcknowledgedColorText = Color.FromArgb(colorFromRegistry);
                if (_alarmNotAcknowledgedColorText.IsEmpty)
                {
                    _alarmNotAcknowledgedColorText = CgpServerGlobals.ALARM_NOT_ACKNOWLEDGED_COLOR_TEXT;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpAlarmNotAcknowledgedColorText, colorFromRegistry);
                registryKey.DeleteValue(CgpServerGlobals.CGP_ALARM_NOT_ACKNOWLEDGED_COLOR_TEXT);
            }
            catch { }

            try
            {
                int colorFromRegistry = (int)registryKey.GetValue(CgpServerGlobals.CGP_ALARM_NOT_ACKNOWLEDGED_COLOR_BACKGROUND);
                _alarmNotAcknowledgedColorBackground = Color.FromArgb(colorFromRegistry);
                if (_alarmNotAcknowledgedColorBackground.IsEmpty)
                {
                    _alarmNotAcknowledgedColorBackground = CgpServerGlobals.ALARM_NOT_ACKNOWLEDGED_COLOR_BACKGROUND;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpAlarmNotAcknowledgedColorBackground, colorFromRegistry);
                registryKey.DeleteValue(CgpServerGlobals.CGP_ALARM_NOT_ACKNOWLEDGED_COLOR_BACKGROUND);
            }
            catch { }

            try
            {
                int colorFromRegistry = (int)registryKey.GetValue(CgpServerGlobals.CGP_ALARM_COLOR_TEXT);
                _alarmColorText = Color.FromArgb(colorFromRegistry);
                if (_alarmColorText.IsEmpty)
                {
                    _alarmColorText = CgpServerGlobals.ALARM_COLOR_TEXT;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpAlarmColorText, colorFromRegistry);
                registryKey.DeleteValue(CgpServerGlobals.CGP_ALARM_COLOR_TEXT);
            }
            catch { }

            try
            {
                int colorFromRegistry = (int)registryKey.GetValue(CgpServerGlobals.CGP_ALARM_COLOR_BACKGROUND);
                _alarmColorBackground = Color.FromArgb(colorFromRegistry);
                if (_alarmColorBackground.IsEmpty)
                {
                    _alarmColorBackground = CgpServerGlobals.ALARM_COLOR_BACKGROUND;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpAlarmColorBackground, colorFromRegistry);
                registryKey.DeleteValue(CgpServerGlobals.CGP_ALARM_COLOR_BACKGROUND);
            }
            catch { }

            try
            {
                int colorFromRegistry = (int)registryKey.GetValue(CgpServerGlobals.CGP_NORMAL_NOT_ACKNOWLEDGED_COLOR_TEXT);
                _normalNotAcknowledgedColorText = Color.FromArgb(colorFromRegistry);
                if (_normalNotAcknowledgedColorText.IsEmpty)
                {
                    _normalNotAcknowledgedColorText = CgpServerGlobals.NORMAL_NOT_ACKNOWLEDGED_COLOR_TEXT;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpNormalNotAcknowledgedColorText, colorFromRegistry);
                registryKey.DeleteValue(CgpServerGlobals.CGP_NORMAL_NOT_ACKNOWLEDGED_COLOR_TEXT);
            }
            catch { }

            try
            {
                int colorFromRegistry = (int)registryKey.GetValue(CgpServerGlobals.CGP_NORMAL_NOT_ACKNOWLEDGED_COLOR_BACKGROUND);
                _normalNotAcknowledgedColorBackground = Color.FromArgb(colorFromRegistry);
                if (_normalNotAcknowledgedColorBackground.IsEmpty)
                {
                    _normalNotAcknowledgedColorBackground = CgpServerGlobals.NORMAL_NOT_ACKNOWLEDGED_COLOR_BACKGROUND;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpNormalNotAcknowledgedColorBackground, colorFromRegistry);
                registryKey.DeleteValue(CgpServerGlobals.CGP_NORMAL_NOT_ACKNOWLEDGED_COLOR_BACKGROUND);
            }
            catch { }

            try
            {
                int colorFromRegistry = (int)registryKey.GetValue(CgpServerGlobals.CGP_NORMAL_COLOR_TEXT);
                _normalColorText = Color.FromArgb(colorFromRegistry);
                if (_normalColorText.IsEmpty)
                {
                    _normalColorText = CgpServerGlobals.NORMAL_COLOR_TEXT;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpNormalColorText, colorFromRegistry);
                registryKey.DeleteValue(CgpServerGlobals.CGP_NORMAL_COLOR_TEXT);
            }
            catch { }

            try
            {
                int colorFromRegistry = (int)registryKey.GetValue(CgpServerGlobals.CGP_NORMAL_COLOR_BACKGROUND);
                _normalColorBackground = Color.FromArgb(colorFromRegistry);
                if (_normalColorBackground.IsEmpty)
                {
                    _normalColorBackground = CgpServerGlobals.NORMAL_COLOR_BACKGROUND;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpNormalColorBackground, colorFromRegistry);
                registryKey.DeleteValue(CgpServerGlobals.CGP_NORMAL_COLOR_BACKGROUND);
            }
            catch { }

            try
            {
                int colorFromRegistry = (int)registryKey.GetValue(CgpServerGlobals.CGP_NO_ALARMS_IN_QUEUE_COLOR_TEXT);
                _noAlarmsInQueueColorText = Color.FromArgb(colorFromRegistry);
                if (_noAlarmsInQueueColorText.IsEmpty)
                {
                    _noAlarmsInQueueColorText = CgpServerGlobals.NO_ALARMS_IN_QUEUE_COLOR_TEXT;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpNoAlarmsInQueueColorText, colorFromRegistry);
                registryKey.DeleteValue(CgpServerGlobals.CGP_NO_ALARMS_IN_QUEUE_COLOR_TEXT);
            }
            catch { }

            try
            {
                int colorFromRegistry = (int)registryKey.GetValue(CgpServerGlobals.CGP_NO_ALARMS_IN_QUEUE_COLOR_BACKGROUND);
                _noAlarmsInQueueColorBackground = Color.FromArgb(colorFromRegistry);
                if (_noAlarmsInQueueColorBackground.IsEmpty)
                {
                    _noAlarmsInQueueColorBackground = CgpServerGlobals.NO_ALARMS_IN_QUEUE_COLOR_BACKGROUND;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpNoAlarmsInQueueColorBackground, colorFromRegistry);
                registryKey.DeleteValue(CgpServerGlobals.CGP_NO_ALARMS_IN_QUEUE_COLOR_BACKGROUND);
            }
            catch { }

            //AutoClose
            try
            {
                _isTurnedOn = Convert.ToBoolean(registryKey.GetValue(CgpServerGlobals.CGP_AUTOCLOSE_TURNED_ON));
                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpAutoCloseTurnedOn, _isTurnedOn);
                registryKey.DeleteValue(CgpServerGlobals.CGP_AUTOCLOSE_TURNED_ON);
            }
            catch { }

            try
            {
                _autoCloseTimeout = (int)registryKey.GetValue(CgpServerGlobals.CGP_AUTOCLOSE_TIMEOUT);
                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpAutoCloseTimeout, _autoCloseTimeout);
                registryKey.DeleteValue(CgpServerGlobals.CGP_AUTOCLOSE_TIMEOUT);
            }
            catch { }

            try
            {
                _changePassDays = (int)registryKey.GetValue(CgpServerGlobals.CGP_CHANGE_PASSWORD_DAYS);
                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpChangePasswordDay, _changePassDays);
                registryKey.DeleteValue(CgpServerGlobals.CGP_CHANGE_PASSWORD_DAYS);
            }
            catch { }

            //security settings
            try
            {
                int? pinRequire = (int?)registryKey.GetValue(CgpServerGlobals.CGP_REQUIRE_PIN_CARD_LOGIN);
                if (pinRequire != null)
                {
                    _requirePINCardLogin = (pinRequire != 0);
                    DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpRequirePINCardLogin, _requirePINCardLogin);
                }

                registryKey.DeleteValue(CgpServerGlobals.CGP_REQUIRE_PIN_CARD_LOGIN);
            }
            catch { }

            try
            {
                _uniqueAndNotNullPersonalKey = Convert.ToBoolean(registryKey.GetValue(CgpServerGlobals.CGP_UNIQUE_NOT_NULL_PERSONAL_KEY));
                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpUniqueNotNullPersonalKey, _uniqueAndNotNullPersonalKey);
                registryKey.DeleteValue(CgpServerGlobals.CGP_UNIQUE_NOT_NULL_PERSONAL_KEY);
            }
            catch { }

            try
            {
                _lockClientApplication = Convert.ToBoolean(registryKey.GetValue(CgpServerGlobals.CGP_LOCK_CLIENT_APPLICATION));
                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpLockClientApplication, _lockClientApplication);
                registryKey.DeleteValue(CgpServerGlobals.CGP_LOCK_CLIENT_APPLICATION);
            }
            catch { }

            try
            {
                int? ccuPassword = (int?)registryKey.GetValue(CgpServerGlobals.CGP_CCU_CONFIGURATION_TO_SERVER_BY_PASSWORD);
                if (ccuPassword == null)
                {
                    _ccuConfigurationToServerByPassword = true;
                }
                else
                {
                    _ccuConfigurationToServerByPassword = (ccuPassword != 0);
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CcuConfigurationToServerByPassword, _ccuConfigurationToServerByPassword);
                registryKey.DeleteValue(CgpServerGlobals.CGP_CCU_CONFIGURATION_TO_SERVER_BY_PASSWORD);
            }
            catch { }

            try
            {
                int? requredSecurePin = (int?)registryKey.GetValue(CgpServerGlobals.CGP_REQUIRED_SECURE_PIN);
                if (requredSecurePin == null)
                {
                    _requiredSecurePin = false;
                }
                else
                {
                    _requiredSecurePin = (requredSecurePin != 0);
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpRequredSecurePin, _requiredSecurePin);
                registryKey.DeleteValue(CgpServerGlobals.CGP_REQUIRED_SECURE_PIN);
            }
            catch { }

            try
            {
                int? disableCcuAssignmnet = (int?)registryKey.GetValue(CgpServerGlobals.CGP_DISABLE_CCU_PLUG_N_PLAY_AUTOMATIC_ASSIGNMENT);
                if (disableCcuAssignmnet == null)
                {
                    _disableCcuPnPAutomaticAssignmnet = false;
                }
                else
                {
                    _disableCcuPnPAutomaticAssignmnet = (disableCcuAssignmnet != 0);
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.DisableCcuPnPAssigmnet, _disableCcuPnPAutomaticAssignmnet);
                registryKey.DeleteValue(CgpServerGlobals.CGP_DISABLE_CCU_PLUG_N_PLAY_AUTOMATIC_ASSIGNMENT);
            }
            catch { }

            try
            {
                int? onlyUnassignedCards = (int?)registryKey.GetValue(CgpServerGlobals.CGP_LIST_ONLY_UNASSIGNED_CARDS_IN_PERSON_FORM);
                if (onlyUnassignedCards == null)
                {
                    _listOnlyUnassignedCardsInPersonForm = false;
                }
                else
                {
                    _listOnlyUnassignedCardsInPersonForm = (onlyUnassignedCards != 0);
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.ListOnlyUnassignedCardsInPersonForm, _listOnlyUnassignedCardsInPersonForm);
                registryKey.DeleteValue(CgpServerGlobals.CGP_LIST_ONLY_UNASSIGNED_CARDS_IN_PERSON_FORM);
            }
            catch { }

            try
            {
                int? delayToSaveAlarms = (int?)registryKey.GetValue(CgpServerGlobals.CGP_DELAY_TO_SAVE_ALARMS_FROM_CARD_READERS);
                if (delayToSaveAlarms == null)
                {
                    _delayToSaveAlarmsFromCardReaders = CgpServerGlobals.DEFAULT_DELAY_TO_SAVE_ALARMS_FROM_CARD_READERS;
                }
                else
                {
                    _delayToSaveAlarmsFromCardReaders = delayToSaveAlarms.Value;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.DelayToSaveAlarmsFromCardReaders, _delayToSaveAlarmsFromCardReaders);
                registryKey.DeleteValue(CgpServerGlobals.CGP_DELAY_TO_SAVE_ALARMS_FROM_CARD_READERS);
            }
            catch { }

            try
            {
                int? eventlogInputStateChanged = (int?)registryKey.GetValue(CgpServerGlobals.CGP_EVENTLOG_INPUT_STATE_CHANGED);
                if (eventlogInputStateChanged == null)
                {
                    _eventlogInputStateChanged = true;
                }
                else
                {
                    _eventlogInputStateChanged = eventlogInputStateChanged != 0;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogInputStateChanged, _eventlogInputStateChanged);
                registryKey.DeleteValue(CgpServerGlobals.CGP_EVENTLOG_INPUT_STATE_CHANGED);
            }
            catch { }

            try
            {
                int? eventlogOutputStateChanged = (int?)registryKey.GetValue(CgpServerGlobals.CGP_EVENTLOG_OUTPUT_STATE_CHANGED);
                if (eventlogOutputStateChanged == null)
                {
                    _eventlogOutputStateChanged = true;
                }
                else
                {
                    _eventlogOutputStateChanged = eventlogOutputStateChanged != 0;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogOutputStateChanged, _eventlogOutputStateChanged);
                registryKey.DeleteValue(CgpServerGlobals.CGP_EVENTLOG_OUTPUT_STATE_CHANGED);
            }
            catch { }

            try
            {
                int? eventlogAlarmAreaAlarmStateChanged = (int?)registryKey.GetValue(CgpServerGlobals.CGP_EVENTLOG_ALRM_AREA_ALARM_STATE_CHANGED);
                if (eventlogAlarmAreaAlarmStateChanged == null)
                {
                    _eventlogAlarmAreaAlarmStateChanged = true;
                }
                else
                {
                    _eventlogAlarmAreaAlarmStateChanged = eventlogAlarmAreaAlarmStateChanged != 0;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogAlarmAreaAlarmStateChanged, _eventlogAlarmAreaAlarmStateChanged);
                registryKey.DeleteValue(CgpServerGlobals.CGP_EVENTLOG_ALRM_AREA_ALARM_STATE_CHANGED);
            }
            catch { }

            try
            {
                int? eventlogAlarmAreaActivationStateChanged = (int?)registryKey.GetValue(CgpServerGlobals.CGP_EVENTLOG_ALRM_AREA_ACTIVATION_STATE_CHANGED);
                if (eventlogAlarmAreaActivationStateChanged == null)
                {
                    _eventlogAlarmAreaActivationStateChanged = true;
                }
                else
                {
                    _eventlogAlarmAreaActivationStateChanged = eventlogAlarmAreaActivationStateChanged != 0;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogAlarmAreaActivationStateChanged, _eventlogAlarmAreaActivationStateChanged);
                registryKey.DeleteValue(CgpServerGlobals.CGP_EVENTLOG_ALRM_AREA_ACTIVATION_STATE_CHANGED);
            }
            catch { }

            try
            {
                int? eventlogCardReaderOnlineStateChanged = (int?)registryKey.GetValue(CgpServerGlobals.CGP_EVENTLOG_CARD_READER_ONLINE_STATE_CHANGED);
                if (eventlogCardReaderOnlineStateChanged == null)
                {
                    _eventlogCardReaderOnlineStateChanged = true;
                }
                else
                {
                    _eventlogCardReaderOnlineStateChanged = eventlogCardReaderOnlineStateChanged != 0;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogCardReaderOnlineStateChanged, _eventlogCardReaderOnlineStateChanged);
                registryKey.DeleteValue(CgpServerGlobals.CGP_EVENTLOG_CARD_READER_ONLINE_STATE_CHANGED);
            }
            catch { }

            try
            {
                registryKey.DeleteValue(CgpServerGlobals.CGP_ADVANCED_DEVICE_SETTINGS_MAX_EVENTS_COUNT_SD_CARD);
            }
            catch { }

            try
            {
                registryKey.DeleteValue(CgpServerGlobals.CGP_ADVANCED_DEVICE_SETTINGS_MAX_EVENTS_COUNT_NAND_FLASH);
            }
            catch { }

            try
            {
                registryKey.DeleteValue(CgpServerGlobals.CGP_ADVANCED_DEVICE_SETTINGS_NO_FLASHING_EVENTS);
            }
            catch { }

            try
            {
                int? maxEventsCountForInsert = (int?)registryKey.GetValue(CgpServerGlobals.CGP_ADVANCED_SETTINGS_MAX_EVENTS_COUNT_FOR_INSERT);
                if (maxEventsCountForInsert == null)
                {
                    _maxEventsCountForInsert = CgpServerGlobals.DEFAULT_MAX_EVENTS_COUNT_FOR_INSERT;
                }
                else
                {
                    _maxEventsCountForInsert = maxEventsCountForInsert.Value;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.AdvancedSettingsMaxEventsCountForInsert, _maxEventsCountForInsert);
                registryKey.DeleteValue(CgpServerGlobals.CGP_ADVANCED_SETTINGS_MAX_EVENTS_COUNT_FOR_INSERT);
            }
            catch { }

            try
            {
                int? delayForSaveEvents = (int?)registryKey.GetValue(CgpServerGlobals.CGP_ADVANCED_SETTINGS_DELAY_FOR_SAVE_EVENTS);
                if (delayForSaveEvents == null)
                {
                    _delayForSaveEvents = CgpServerGlobals.DEFAULT_DELAY_FOR_SAVE_EVENTS;
                }
                else
                {
                    _delayForSaveEvents = delayForSaveEvents.Value;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.AdvancedSettingsDelayForSaveEvents, _delayForSaveEvents);
                registryKey.DeleteValue(CgpServerGlobals.CGP_ADVANCED_SETTINGS_DELAY_FOR_SAVE_EVENTS);
            }
            catch { }

            try
            {
                int? syncingTimeFromServer = (int?)registryKey.GetValue(CgpServerGlobals.CGP_ADVANCED_DEVICE_SETTINGS_SYNCING_TIME_FROM_SERVER);
                if (syncingTimeFromServer == null)
                {
                    _syncingTimeFromServer = false;
                }
                else
                {
                    _syncingTimeFromServer = syncingTimeFromServer != 0;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.AdvancedDeviceSettingsSyncingTimeFromServer, _syncingTimeFromServer);
                registryKey.DeleteValue(CgpServerGlobals.CGP_ADVANCED_DEVICE_SETTINGS_SYNCING_TIME_FROM_SERVER);
            }
            catch { }

            try
            {
                int? periodOfTimeSyncWithoutStratum = (int?)registryKey.GetValue(CgpServerGlobals.CGP_ADVANCED_DEVICE_SETTINGS_PERIOD_OF_TIME_SYNC_WITHOUT_STRATUM);
                if (periodOfTimeSyncWithoutStratum == null)
                {
                    _periodOfTimeSyncWithoutStratum = CgpServerGlobals.DEFAULT_PERIOD_OF_TIME_SYNC_WITHOUT_STRATUM;
                }
                else
                {
                    _periodOfTimeSyncWithoutStratum = periodOfTimeSyncWithoutStratum.Value;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.AdvancedDeviceSettingsPeriodOfTimeSyncWithoutStratum, _periodOfTimeSyncWithoutStratum);
                registryKey.DeleteValue(CgpServerGlobals.CGP_ADVANCED_DEVICE_SETTINGS_PERIOD_OF_TIME_SYNC_WITHOUT_STRATUM);
            }
            catch { }

            try
            {
                int? periodicTimeSyncTolerance = (int?)registryKey.GetValue(CgpServerGlobals.CGP_ADVANCED_DEVICE_SETTINGS_PERIODIC_TIME_SYNC_TOLERANCE);
                if (periodicTimeSyncTolerance == null)
                {
                    _periodicTimeSyncTolerance = CgpServerGlobals.DEFAULT_PERIODIC_TIME_SYNC_TOLERANCE;
                }
                else
                {
                    _periodicTimeSyncTolerance = periodicTimeSyncTolerance.Value;
                }

                DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.AdvancedDeviceSettingsPeriodicTimeSyncTolerance, _periodicTimeSyncTolerance);
                registryKey.DeleteValue(CgpServerGlobals.CGP_ADVANCED_DEVICE_SETTINGS_PERIODIC_TIME_SYNC_TOLERANCE);
            }
            catch { }

            DatabaseGeneralOptions.Singleton.SetSavedValuesFromRegistryToDatabase();
        }

        /// <summary>
        /// Save settings to registry
        /// </summary>
        public void SaveSettingsToRegistry()
        {
            RegistryKey rk = RegistryHelper.GetOrAddKey(CgpServerGlobals.REGISTRY_GENERAL_SETTINGS);

            if (null == rk)
            {
                _isConfigured = false;
                throw new DoesNotExistException(CgpServerGlobals.REGISTRY_GENERAL_SETTINGS);
            }

            bool continueWithOptional = true;

            try
            {
                // most important
                rk.SetValue(CgpServerGlobals.CGP_REMOTING_SERVER_PORT, _remotingServerPort, RegistryValueKind.DWord);
                rk.SetValue(CgpServerGlobals.CGP_SERVER_LICENCE_PATH, _licencePath, RegistryValueKind.String);
                rk.SetValue(CgpServerGlobals.CGP_REMOTING_SERVER_IP_ADDRESS, _remotingServerIpAddress ?? String.Empty, RegistryValueKind.String);
            }
            catch
            {
                _isConfigured = false;
                continueWithOptional = false;
            }

            if (continueWithOptional)
            {
                // optional
                rk.SetValue(CgpServerGlobals.CGP_SERVER_CURRENT_LANGUAGE, _currentLanguage ?? CgpServerGlobals.DEFAULT_LANGUAGE, RegistryValueKind.String);
                rk.SetValue(CgpServerGlobals.CGP_SERVER_FRIENDLY_NAME, _friendlyName, RegistryValueKind.String);

                _isConfigured = true;
            }
        }

        public void SaveAutoStartDHCP()
        {
            if (!DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.AutoStartDHCP, _autoStartDHCP))
            {
                throw new Exception("Save auto close settings to database failed");
            }
        }

        public void SaveSMTP()
        {
            if (!DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SmtpServer, _smtpServer) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SmtpPort, _smtpPort) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SmtpSourceEmailAddress, _smtpSourceEmailAddress) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SmtpSubject, _smtpSubject) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SmtpCredentials, _smtpCredentials) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SmtpSsl, _smtpSsl))
            {
                throw new Exception("Save SMTP settings to database failed");
            }
        }

        public void SaveSerialPort()
        {
            if (!DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SerialPort, _serialPort) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SerialPortBaudRate, _serialPortBaudRate) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SerialPortDataBits, _serialPortDataBits) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SerialPortParity, _serailPortParity) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SerialPortStopBits, _serialPortStopBits) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SerialPortFlowControl, _serialPortFlowControl) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SerialPortParityCheck, _serialPortParityCheck) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SerialPortCarrierDetect, _serialPortCarrierDetect) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SerialPortPin, _serialPortPin))
            {
                throw new Exception("Save eventlogs expiration settings to database failed");
            }
        }

        public void SaveDatabaseBackup()
        {
            if (!DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.DatabaseBackupTimeZone, _timeZoneGuidString) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.DatabaseBackupPath, _databaseBackupPath))
            {
                throw new Exception("Save database backup settings to database failed");
            }
        }

        public void SaveDatabaseEventlogExpiration()
        {
            if (!DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogsExpirationTimeZone, _eventlogTimeZoneGuidString) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogsExpirationDays, _eventlogsExpirationDays) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogsMaxCountValue, _eventlogsMaxCountValue) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogsMaxCountExponent, _eventlogsMaxCountExponent))
            {
                throw new Exception("Save eventlogs expiration settings to database failed");
            }
        }

        public void SaveDatabaseCustomerAndSupplierInfo()
        {
            if (!DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CustomerCityState, CustomerCityState) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CustomerCompanyName, CustomerCompanyName) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CustomerContactPerson, CustomerContactPerson) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CustomerCountry, CustomerCountry) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CustomerDeliveryAddress, CustomerDeliveryAddress) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CustomerPhone, CustomerPhone) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CustomerWebsite, CustomerWebsite) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CustomerZipCode, CustomerZipCode) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SupplierCityState, SupplierCityState) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SupplierCompanyName, SupplierCompanyName) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SupplierContactPerson, SupplierContactPerson) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SupplierCountry, SupplierCountry) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SupplierDeliveryAddress, SupplierDeliveryAddress) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SupplierPhone, SupplierPhone) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SupplierWebsite, SupplierWebsite) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.SupplierZipCode, SupplierZipCode))
            {
                throw new Exception("Save Customer and Supplier info to database failed");
            }
        }

        public void SaveColorSetting()
        {
            int? intDragDropColorText = null;
            if (_dragDropColorText != null && !_dragDropColorText.IsEmpty)
                intDragDropColorText = _dragDropColorText.ToArgb();

            int? intDragDropColorBackground = null;
            if (_dragDropColorBackground != null && !_dragDropColorBackground.IsEmpty)
                intDragDropColorBackground = _dragDropColorBackground.ToArgb();

            int? intReferenceObjectColorText = null;
            if (_referenceObjectColorText != null && !_referenceObjectColorText.IsEmpty)
                intReferenceObjectColorText = _referenceObjectColorText.ToArgb();

            int? intReferenceObjectColorBackground = null;
            if (_referenceObjectColorBackground != null && !_referenceObjectColorBackground.IsEmpty)
                intReferenceObjectColorBackground = _referenceObjectColorBackground.ToArgb();

            int? intAlarmNotAcknowledgedColorText = null;
            if (_alarmNotAcknowledgedColorText != null && !_alarmNotAcknowledgedColorText.IsEmpty)
                intAlarmNotAcknowledgedColorText = _alarmNotAcknowledgedColorText.ToArgb();

            int? intAlarmNotAcknowledgedColorBackground = null;
            if (_alarmNotAcknowledgedColorBackground != null && !_alarmNotAcknowledgedColorBackground.IsEmpty)
                intAlarmNotAcknowledgedColorBackground = _alarmNotAcknowledgedColorBackground.ToArgb();

            int? intAlarmColorText = null;
            if (_alarmColorText != null && !_alarmColorText.IsEmpty)
                intAlarmColorText = _alarmColorText.ToArgb();

            int? intAlarmColorBackground = null;
            if (_alarmColorBackground != null && !_alarmColorBackground.IsEmpty)
                intAlarmColorBackground = _alarmColorBackground.ToArgb();

            int? intNormalNotAcknowledgedColorText = null;
            if (_normalNotAcknowledgedColorText != null && !_normalNotAcknowledgedColorText.IsEmpty)
                intNormalNotAcknowledgedColorText = _normalNotAcknowledgedColorText.ToArgb();

            int? intNormalNotAcknowledgedColorBackground = null;
            if (_normalNotAcknowledgedColorBackground != null && !_normalNotAcknowledgedColorBackground.IsEmpty)
                intNormalNotAcknowledgedColorBackground = _normalNotAcknowledgedColorBackground.ToArgb();

            int? intNormalColorText = null;
            if (_normalColorText != null && !_normalColorText.IsEmpty)
                intNormalColorText = _normalColorText.ToArgb();

            int? intNormalColorBackground = null;
            if (_normalColorBackground != null && !_normalColorBackground.IsEmpty)
                intNormalColorBackground = _normalColorBackground.ToArgb();

            int? intNoAlarmsInQueueColorText = null;
            if (_noAlarmsInQueueColorText != null && !_noAlarmsInQueueColorText.IsEmpty)
                intNoAlarmsInQueueColorText = _noAlarmsInQueueColorText.ToArgb();

            int? intNoAlarmsInQueueColorBackground = null;
            if (_noAlarmsInQueueColorBackground != null && !_noAlarmsInQueueColorBackground.IsEmpty)
                intNoAlarmsInQueueColorBackground = _noAlarmsInQueueColorBackground.ToArgb();

            if ((intDragDropColorText != null && !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpDragDropColorText, intDragDropColorText.Value)) ||
                (intDragDropColorBackground != null && !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpDragDropColorBackground, intDragDropColorBackground.Value)) ||
                (intReferenceObjectColorText != null && !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpReferenceObjectColorText, intReferenceObjectColorText.Value)) ||
                (intReferenceObjectColorBackground != null && !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpReferenceObjectColorBackground, intReferenceObjectColorBackground.Value)) ||
                (intAlarmNotAcknowledgedColorText != null && !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpAlarmNotAcknowledgedColorText, intAlarmNotAcknowledgedColorText.Value)) ||
                (intAlarmNotAcknowledgedColorBackground != null && !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpAlarmNotAcknowledgedColorBackground, intAlarmNotAcknowledgedColorBackground.Value)) ||
                (intAlarmColorText != null && !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpAlarmColorText, intAlarmColorText.Value)) ||
                (intAlarmColorBackground != null && !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpAlarmColorBackground, intAlarmColorBackground.Value)) ||
                (intNormalNotAcknowledgedColorText != null && !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpNormalNotAcknowledgedColorText, intNormalNotAcknowledgedColorText.Value)) ||
                (intNormalNotAcknowledgedColorBackground != null && !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpNormalNotAcknowledgedColorBackground, intNormalNotAcknowledgedColorBackground.Value)) ||
                (intNormalColorText != null && !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpNormalColorText, intNormalColorText.Value)) ||
                (intNormalColorBackground != null && !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpNormalColorBackground, intNormalColorBackground.Value)) ||
                (intNoAlarmsInQueueColorText != null && !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpNoAlarmsInQueueColorText, intNoAlarmsInQueueColorText.Value)) ||
                (intNoAlarmsInQueueColorBackground != null && !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpNoAlarmsInQueueColorBackground, intNoAlarmsInQueueColorBackground.Value)))
            {
                throw new Exception("Save color settings to database failed");
            }
        }

        public void SaveAutoCloseSetting()
        {
            if (!DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpAutoCloseTurnedOn, _isTurnedOn) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpAutoCloseTimeout, _autoCloseTimeout))
            {
                throw new Exception("Save auto close settings to database failed");
            }
        }

        public void SaveSecuritySettings()
        {
            if (!DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpChangePasswordDay, _changePassDays)
                || !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpRequirePINCardLogin, _requirePINCardLogin)
                || !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpUniqueNotNullPersonalKey, _uniqueAndNotNullPersonalKey)
                || !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpLockClientApplication, _lockClientApplication)
                || !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CcuConfigurationToServerByPassword, _ccuConfigurationToServerByPassword)
                || !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CgpRequredSecurePin, _requiredSecurePin)
                || !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.DisableCcuPnPAssigmnet, _disableCcuPnPAutomaticAssignmnet)
                || !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.ListOnlyUnassignedCardsInPersonForm, _listOnlyUnassignedCardsInPersonForm)
                || !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.DelayToSaveAlarmsFromCardReaders, _delayToSaveAlarmsFromCardReaders)
                || !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.UniqueAKeyCSRestriction, _uniqueAKeyCSRestriction)
                || !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.CardReadersAllowPINCachingInMenu, _cardReadersAllowPINCachingInMenu)
                || !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.MinimalCodeLength, _minimalCodeLength)
                || !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.MaximalCodeLength, _maximalCodeLength)
                || !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.IsPinConfirmationObligatory, _isPinConfirmationObligatory)
                )
            {
                throw new Exception("Save security settings to database failed");
            }
        }

        public void SaveEventlogs()
        {
            if (!DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogInputStateChanged, _eventlogInputStateChanged) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogOutputStateChanged, _eventlogOutputStateChanged) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogAlarmAreaAlarmStateChanged, _eventlogAlarmAreaAlarmStateChanged) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogAlarmAreaActivationStateChanged, _eventlogAlarmAreaActivationStateChanged) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogCardReaderOnlineStateChanged, _eventlogCardReaderOnlineStateChanged) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventSourcesReverseOrder, _eventSourcesReverseOrder) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogReportsTimezoneGuidString, _eventlogReportsTimeZoneGuidString) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.EventlogReportsEmails, _eventlogReportsEmails))
            {
                throw new Exception("Save eventlog settings to database failed");
            }
        }

        public void SaveAdvancedAccessSettings()
        {
            if (!DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.AdvancedDeviceSettingsEnableLoggingSDPSTZChanges, _enableLoggingSDPSTZChanges) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.AdvancedDeviceSettingsSyncingTimeFromServer, _syncingTimeFromServer) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.AdvancedDeviceSettingsPeriodOfTimeSyncWithoutStratum, _periodOfTimeSyncWithoutStratum) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.AdvancedDeviceSettingsPeriodicTimeSyncTolerance, _periodicTimeSyncTolerance) ||
                !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.AlarmAreaRestrictivePolicyForTimeBuying, _alarmAreaRestrictivePolicyForTimeBuying))
            {
                throw new Exception("Save advaced access settings to database failed");
            }
        }

        public void SaveAdvancedSettings()
        {
            if (!DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.AdvancedSettingsMaxEventsCountForInsert, _maxEventsCountForInsert)
                || !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.AdvancedSettingsDelayForSaveEvents, _delayForSaveEvents)
                || !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.AdvancedSettingsClientSessionTimeout, _clientSessionTimeOut)
                || !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.AlarmListSuspendedRefreshTimeout, _alarmListSuspendedRefreshTimeout)
                || !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.AdvancedSettingsCorrectDeserializationFailures, _correctDeserializationFailures)
                || !DatabaseGeneralOptions.Singleton.Set(DatabaseGeneralOptionType.AdvancedSettingsDelayForSendingChangesToCcu, _delayForSendingChangesToCcu))
            {
                throw new Exception("Save advaced settings to database failed");
            }
        }

        public bool IsSetSMTP()
        {
            if (string.IsNullOrEmpty(_smtpServer)) return false;
            if (string.IsNullOrEmpty(_smtpSourceEmailAddress)) return false;

            return true;
        }

        public bool IsSetSerialPort()
        {
            if (string.IsNullOrEmpty(_serialPort)) return false;
            if (string.IsNullOrEmpty(_serailPortParity)) return false;
            if (string.IsNullOrEmpty(_serialPortStopBits)) return false;
            if (string.IsNullOrEmpty(_serialPortFlowControl)) return false;
            if (_serialPortPin == null) return false;
            return false;
        }

        public bool ShowHiddenFeature(string featureName)
        {
            return _showHiddenFeatures != null && _showHiddenFeatures.Contains(featureName);
        }
    }
}
