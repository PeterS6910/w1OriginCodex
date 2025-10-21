using System;
using System.Collections.Generic;
using System.Drawing;

using Contal.IwQuick;
using Contal.IwQuick.Remoting;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    public class ServerGeneralOptions
    {
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

        //Color settings
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

        //Security Setting 
        private int _changePassDays;
        private bool? _requirePINCardLogin;
        private bool _uniqueAndNotNull;
        private bool _lockClientApplication;
        private bool _ccuConfigurationToServerByPassword;
        private bool _requiredSecurePin;
        private bool _disableCcuPnPAutomaticAssignmnet;
        private bool _listOnlyUnassignedCardsInPersonForm;
        private int _delayToSaveAlarmsFromCardReaders;
        private bool _uniqueAKeyCSRestriction;
        private bool _cardReadersAllowPINCachingInMenu;

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

        //Advanced settings
        private int _maxEventsCountForInsert;
        private int _delayForSaveEvents;
        private int _clientSessionTimeOut;
        private int _alarmListSuspendedRefreshTimeout;
        private bool _correctDeserializationFailures;

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

        public bool UniqueAndNotNull
        {
            get { return _uniqueAndNotNull; }
            set { _uniqueAndNotNull = value; }
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

        public bool RequiredSecurePin
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

        public int MinimalCodeLength { get; set; }

        public int MaximalCodeLength { get; set; }

        public bool IsPinConfirmationObligatory { get; set; }

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

        public bool EventSourcesReverseOrder
        {
            get { return _eventSourcesReverseOrder; }
            set { _eventSourcesReverseOrder = value; }
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

        public int DelayForSendingChangesToCcu { get; set; }

        public bool AlarmAreaRestrictivePolicyForTimeBuying { get; set; }
    }

    public class ColorSettingsChangedHandler : ARemotingCallbackHandler
    {
        private static volatile ColorSettingsChangedHandler _singleton = null;
        private static readonly object _syncRoot = new object();

        private DVoid2Void _colorChanged;

        public static ColorSettingsChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new ColorSettingsChangedHandler();
                    }

                return _singleton;
            }
        }

        public ColorSettingsChangedHandler()
            : base("ColorSettingsChangedHandler")
        {
        }

        public void RegisterColorChanged(DVoid2Void colorChanged)
        {
            _colorChanged += colorChanged;
        }

        public void UnregisterColorChanged(DVoid2Void colorChanged)
        {
            _colorChanged -= colorChanged;
        }

        public void RunEvent()
        {
            if (_colorChanged != null)
            {
                try
                {
                    _colorChanged();
                }
                catch
                { }
            }
        }
    }

    public class CustomerAndSupplierInfoChangedHandler : ARemotingCallbackHandler
    {
        private static volatile CustomerAndSupplierInfoChangedHandler _singleton = null;
        private static readonly object _syncRoot = new object();

        private DVoid2Void _infoChanged;

        public static CustomerAndSupplierInfoChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new CustomerAndSupplierInfoChangedHandler();
                    }

                return _singleton;
            }
        }

        public CustomerAndSupplierInfoChangedHandler()
            : base("CustomerAndSupplierInfoChangedHandler")
        {
        }

        public void RegisterInfoChanged(DVoid2Void infoChanged)
        {
            _infoChanged += infoChanged;
        }

        public void UnregisterInfoChanged(DVoid2Void infoChanged)
        {
            _infoChanged -= infoChanged;
        }

        public void RunEvent()
        {
            if (_infoChanged != null)
            {
                try
                {
                    _infoChanged();
                }
                catch
                { }
            }
        }
    }

    public class AutoCloseSettingsChangedHandler : ARemotingCallbackHandler
    {
        private static volatile AutoCloseSettingsChangedHandler _singleton = null;
        private static readonly object _syncRoot = new object();

        private DVoid2Void _autoCloseChanged;

        public static AutoCloseSettingsChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new AutoCloseSettingsChangedHandler();
                    }

                return _singleton;
            }
        }

        public AutoCloseSettingsChangedHandler()
            : base("AutoCloseSettingsChangedHandler")
        {
        }

        public void RegisterAutoCloseChanged(DVoid2Void autoCloseChanged)
        {
            _autoCloseChanged += autoCloseChanged;
        }

        public void UnregisterAutoCloseChanged(DVoid2Void autoCloseChanged)
        {
            _autoCloseChanged -= autoCloseChanged;
        }

        public void RunEvent()
        {
            if (_autoCloseChanged != null)
            {
                try
                {
                    _autoCloseChanged();
                }
                catch
                { }
            }
        }
    }

    public class SqlServerOnlineStateChangedHandler : ARemotingCallbackHandler
    {
        private static volatile SqlServerOnlineStateChangedHandler _singleton = null;
        private static readonly object _syncRoot = new object();

        private Action<bool> _sqlServerOnlineStateChanged;

        public static SqlServerOnlineStateChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new SqlServerOnlineStateChangedHandler();
                    }

                return _singleton;
            }
        }

        public SqlServerOnlineStateChangedHandler()
            : base("SqlServerOnlineStateChangedHandler")
        {
        }

        public void RegisterSqlServerOnlineStateChanged(Action<bool> sqlServerOnlineStateChanged)
        {
            _sqlServerOnlineStateChanged += sqlServerOnlineStateChanged;
        }

        public void UnregisterSqlServerOnlineStateChanged(Action<bool> sqlServerOnlineStateChanged)
        {
            _sqlServerOnlineStateChanged -= sqlServerOnlineStateChanged;
        }

        public void RunEvent(bool isOnline)
        {
            if (_sqlServerOnlineStateChanged != null)
            {
                try
                {
                    _sqlServerOnlineStateChanged(isOnline);
                }
                catch
                { }
            }
        }
    }

    public class DatabaseBackupSettingsChangedHandler : ARemotingCallbackHandler
    {
        private static volatile DatabaseBackupSettingsChangedHandler _singleton = null;
        private static readonly object _syncRoot = new object();

        private DVoid2Void _databaseBackupSettingsChanged;

        public static DatabaseBackupSettingsChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new DatabaseBackupSettingsChangedHandler();
                    }

                return _singleton;
            }
        }

        public DatabaseBackupSettingsChangedHandler()
            : base("DatabaseBackupSettingsChangedHandler")
        {
        }

        public void RegisterDatabaseBackupSettingsChanged(DVoid2Void databaseBackupSettingsChanged)
        {
            _databaseBackupSettingsChanged += databaseBackupSettingsChanged;
        }

        public void UnregisterDatabaseBackupSettingsChanged(DVoid2Void databaseBackupSettingsChanged)
        {
            _databaseBackupSettingsChanged -= databaseBackupSettingsChanged;
        }

        public void RunEvent()
        {
            if (_databaseBackupSettingsChanged != null)
            {
                try
                {
                    _databaseBackupSettingsChanged();
                }
                catch
                { }
            }
        }
    }

    public class DatabaseExpirationEventlogSettingsChangedHandler : ARemotingCallbackHandler
    {
        private static volatile DatabaseExpirationEventlogSettingsChangedHandler _singleton = null;
        private static readonly object _syncRoot = new object();

        private DVoid2Void _databaseExpirationEventlogSettingsChanged;

        public static DatabaseExpirationEventlogSettingsChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new DatabaseExpirationEventlogSettingsChangedHandler();
                    }

                return _singleton;
            }
        }

        public DatabaseExpirationEventlogSettingsChangedHandler()
            : base("DatabaseExpirationEventlogSettingsChangedHandler")
        {
        }

        public void RegisterDatabaseExpirationEventlogSettingsChanged(DVoid2Void databaseExpirationEventlogSettingsChanged)
        {
            _databaseExpirationEventlogSettingsChanged += databaseExpirationEventlogSettingsChanged;
        }

        public void UnregisterDatabaseExpirationEventlogSettingsChanged(DVoid2Void databaseExpirationEventlogSettingsChanged)
        {
            _databaseExpirationEventlogSettingsChanged -= databaseExpirationEventlogSettingsChanged;
        }

        public void RunEvent()
        {
            if (_databaseExpirationEventlogSettingsChanged != null)
            {
                try
                {
                    _databaseExpirationEventlogSettingsChanged();
                }
                catch
                { }
            }
        }
    }

    public class DhcpServerSettingsChangedHandler : ARemotingCallbackHandler
    {
        private static volatile DhcpServerSettingsChangedHandler _singleton = null;
        private static readonly object _syncRoot = new object();

        private DVoid2Void _dhcpServerSettingsChanged;

        public static DhcpServerSettingsChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new DhcpServerSettingsChangedHandler();
                    }

                return _singleton;
            }
        }

        public DhcpServerSettingsChangedHandler()
            : base("DhcpServerSettingsChangedHandler")
        {
        }

        public void RegisterDhcpServerSettingsChanged(DVoid2Void dhcpServerSettingsChanged)
        {
            _dhcpServerSettingsChanged += dhcpServerSettingsChanged;
        }

        public void UnregisterDhcpServerSettingsChanged(DVoid2Void dhcpServerSettingsChanged)
        {
            _dhcpServerSettingsChanged -= dhcpServerSettingsChanged;
        }

        public void RunEvent()
        {
            if (_dhcpServerSettingsChanged != null)
            {
                try
                {
                    _dhcpServerSettingsChanged();
                }
                catch
                { }
            }
        }
    }

    public class SecuritySettingsChangedHandler : ARemotingCallbackHandler
    {
        private static volatile SecuritySettingsChangedHandler _singleton = null;
        private static readonly object _syncRoot = new object();

        private DVoid2Void _securitySettingsChanged;

        public static SecuritySettingsChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new SecuritySettingsChangedHandler();
                    }

                return _singleton;
            }
        }

        public SecuritySettingsChangedHandler()
            : base("SecuritySettingsChangedHandler")
        {
        }

        public void RegisterSecuritySettingsChanged(DVoid2Void securitySettingsChanged)
        {
            _securitySettingsChanged += securitySettingsChanged;
        }

        public void UnregisterSecuritySettingsChanged(DVoid2Void securitySettingsChanged)
        {
            _securitySettingsChanged -= securitySettingsChanged;
        }

        public void RunEvent()
        {
            if (_securitySettingsChanged != null)
            {
                try
                {
                    _securitySettingsChanged();
                }
                catch
                { }
            }
        }
    }

    public class AlarmSettingsChangedHandler : ARemotingCallbackHandler
    {
        private static volatile AlarmSettingsChangedHandler _singleton = null;
        private static readonly object _syncRoot = new object();

        private DVoid2Void _alarmSettingsChanged;

        public static AlarmSettingsChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new AlarmSettingsChangedHandler();
                    }

                return _singleton;
            }
        }

        public AlarmSettingsChangedHandler()
            : base("AlarmSettingsChangedHandler")
        {
        }

        public void RegisterAlarmSettingsChanged(DVoid2Void alarmSettingsChanged)
        {
            _alarmSettingsChanged += alarmSettingsChanged;
        }

        public void UnregisterAlarmSettingsChanged(DVoid2Void alarmSettingsChanged)
        {
            _alarmSettingsChanged -= alarmSettingsChanged;
        }

        public void RunEvent()
        {
            if (_alarmSettingsChanged != null)
            {
                try
                {
                    _alarmSettingsChanged();
                }
                catch
                { }
            }
        }
    }

    public class EventlogsChangedHandler : ARemotingCallbackHandler
    {
        private static volatile EventlogsChangedHandler _singleton = null;
        private static readonly object _syncRoot = new object();

        private DVoid2Void _eventlogsChanged;

        public static EventlogsChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new EventlogsChangedHandler();
                    }

                return _singleton;
            }
        }

        public EventlogsChangedHandler()
            : base("EventlogsChangedHandler")
        {
        }

        public void RegisterEventlogsChanged(DVoid2Void eventlogsChanged)
        {
            _eventlogsChanged += eventlogsChanged;
        }

        public void UnregisterEventlogsChanged(DVoid2Void eventlogsChanged)
        {
            _eventlogsChanged -= eventlogsChanged;
        }

        public void RunEvent()
        {
            if (_eventlogsChanged != null)
            {
                try
                {
                    _eventlogsChanged();
                }
                catch
                { }
            }
        }
    }

    public class RemoteServicesSettingsChangedHandler : ARemotingCallbackHandler
    {
        private static volatile RemoteServicesSettingsChangedHandler _singleton = null;
        private static readonly object _syncRoot = new object();

        private DVoid2Void _remoteServicesSettingsChanged;

        public static RemoteServicesSettingsChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new RemoteServicesSettingsChangedHandler();
                    }

                return _singleton;
            }
        }

        public RemoteServicesSettingsChangedHandler()
            : base("RemoteServicesSettingsChangedHandler")
        {
        }

        public void RegisterRemoteServicesSettingsChanged(DVoid2Void remoteServicesSettingsChanged)
        {
            _remoteServicesSettingsChanged += remoteServicesSettingsChanged;
        }

        public void UnregisterRemoteServicesSettingsChanged(DVoid2Void remoteServicesSettingsChanged)
        {
            _remoteServicesSettingsChanged -= remoteServicesSettingsChanged;
        }

        public void RunEvent()
        {
            if (_remoteServicesSettingsChanged != null)
            {
                try
                {
                    _remoteServicesSettingsChanged();
                }
                catch
                { }
            }
        }
    }

    public class SerialPortSettingsChangedHandler : ARemotingCallbackHandler
    {
        private static volatile SerialPortSettingsChangedHandler _singleton = null;
        private static readonly object _syncRoot = new object();

        private DVoid2Void _serialPortSettingsChanged;

        public static SerialPortSettingsChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new SerialPortSettingsChangedHandler();
                    }

                return _singleton;
            }
        }

        public SerialPortSettingsChangedHandler()
            : base("SerialPortSettingsChangedHandler")
        {
        }

        public void RegisterSerialPortSettingsChanged(DVoid2Void serialPortSettingsChanged)
        {
            _serialPortSettingsChanged += serialPortSettingsChanged;
        }

        public void UnregisterSerialPortSettingsChanged(DVoid2Void serialPortSettingsChanged)
        {
            _serialPortSettingsChanged -= serialPortSettingsChanged;
        }

        public void RunEvent()
        {
            if (_serialPortSettingsChanged != null)
            {
                try
                {
                    _serialPortSettingsChanged();
                }
                catch
                { }
            }
        }
    }

    public class AdvancedAccessSettingsChangedHandler : Contal.IwQuick.Remoting.ARemotingCallbackHandler
    {
        private static volatile AdvancedAccessSettingsChangedHandler _singleton = null;
        private static readonly object _syncRoot = new object();

        private DVoid2Void _advancedAccessSettingsChanged;

        public static AdvancedAccessSettingsChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new AdvancedAccessSettingsChangedHandler();
                    }

                return _singleton;
            }
        }

        public AdvancedAccessSettingsChangedHandler()
            : base("AdvancedAccessSettingsChangedHandler")
        {
        }

        public void RegisterAdvancedAccessSettingsChanged(DVoid2Void advancedAccessSettingsChanged)
        {
            _advancedAccessSettingsChanged += advancedAccessSettingsChanged;
        }

        public void UnregisterAdvancedAccessSettingsChanged(DVoid2Void advancedAccessSettingsChanged)
        {
            _advancedAccessSettingsChanged -= advancedAccessSettingsChanged;
        }

        public void RunEvent()
        {
            if (_advancedAccessSettingsChanged != null)
            {
                try
                {
                    _advancedAccessSettingsChanged();
                }
                catch
                { }
            }
        }
    }

    public class AdvancedSettingsChangedHandler : ARemotingCallbackHandler
    {
        private static volatile AdvancedSettingsChangedHandler _singleton = null;
        private static readonly object _syncRoot = new object();

        private DVoid2Void _advancedSettingsChanged;

        public static AdvancedSettingsChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new AdvancedSettingsChangedHandler();
                    }

                return _singleton;
            }
        }

        public AdvancedSettingsChangedHandler()
            : base("AdvancedSettingsChangedHandler")
        {
        }

        public void RegisterAdvancedSettingsChanged(DVoid2Void advancedSettingsChanged)
        {
            _advancedSettingsChanged += advancedSettingsChanged;
        }

        public void UnregisterAdvancedSettingsChanged(DVoid2Void advancedSettingsChanged)
        {
            _advancedSettingsChanged -= advancedSettingsChanged;
        }

        public void RunEvent()
        {
            if (_advancedSettingsChanged != null)
            {
                try
                {
                    _advancedSettingsChanged();
                }
                catch
                { }
            }
        }
    }

    public class WarningSetMaxEventsCountHandler : ARemotingCallbackHandler
    {
        private static volatile WarningSetMaxEventsCountHandler _singleton = null;
        private static readonly object _syncRoot = new object();

        private Action<List<string>> _warningSetMaxEventsCount;

        public static WarningSetMaxEventsCountHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new WarningSetMaxEventsCountHandler();
                    }

                return _singleton;
            }
        }

        public WarningSetMaxEventsCountHandler()
            : base("WarningSetMaxEventsCountHandler")
        {
        }

        public void RegisterWarningSetMaxEventsCount(Action<List<string>> warningSetMaxEventsCount)
        {
            _warningSetMaxEventsCount += warningSetMaxEventsCount;
        }

        public void UnregisterWarningSetMaxEventsCount(Action<List<string>> warningSetMaxEventsCount)
        {
            _warningSetMaxEventsCount -= warningSetMaxEventsCount;
        }

        public void RunEvent(List<string> ccuNames)
        {
            if (_warningSetMaxEventsCount != null)
            {
                try
                {
                    _warningSetMaxEventsCount(ccuNames);
                }
                catch
                { }
            }
        }
    }

    public class DemoPerionHasExpiredHandler : ARemotingCallbackHandler
    {
        private static volatile DemoPerionHasExpiredHandler _singleton = null;
        private static readonly object _syncRoot = new object();

        private DVoid2Void _eventDemoPeriodHasExpired;

        public static DemoPerionHasExpiredHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new DemoPerionHasExpiredHandler();
                    }

                return _singleton;
            }
        }

        public DemoPerionHasExpiredHandler()
            : base("DemoPerionHasExpiredHandler")
        {
        }

        public void RegisterDemoPerionHasExpired(DVoid2Void eventDemoPeriodHasExpired)
        {
            _eventDemoPeriodHasExpired += eventDemoPeriodHasExpired;
        }

        public void UnregisterDemoPerionHasExpired(DVoid2Void eventDemoPeriodHasExpired)
        {
            _eventDemoPeriodHasExpired -= eventDemoPeriodHasExpired;
        }

        public void RunEvent()
        {
            if (_eventDemoPeriodHasExpired != null)
            {
                try
                {
                    _eventDemoPeriodHasExpired();
                }
                catch
                { }
            }
        }
    }
}

