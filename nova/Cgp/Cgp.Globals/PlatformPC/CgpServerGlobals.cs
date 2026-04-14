using System.Drawing;

namespace Contal.Cgp.Globals
{
    public class CgpServerGlobals
    {
        private static CgpServerGlobals _singleton;
        private static readonly object _syncRoot = new object();

        private CgpServerGlobals()
        {

        }

        public static CgpServerGlobals Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new CgpServerGlobals();
                    }

                return _singleton;
            }
        }

        /// <summary>
        /// number of seconds to wait for main service thread termination
        /// </summary>       
        public const int TERMINATION_WAIT_TIME = 30;

        public static readonly Color DRAG_DROP_COLOR_TEXT = Color.Black;
        public static readonly Color DRAG_DROP_COLOR_BACKGROUND = Color.FromArgb(255, 128, 255, 255); //Color.GreenYellow;
        public static readonly Color REFERENCE_OBJECT_COLOR_TEXT = Color.Black;
        public static readonly Color REFERENCE_OBJECT_COLOR_BACKGROUND = Color.FromArgb(255, 128, 255, 128); //Color.LightBlue;
        public static readonly Color ALARM_NOT_ACKNOWLEDGED_COLOR_TEXT = Color.Black;
        public static readonly Color ALARM_NOT_ACKNOWLEDGED_COLOR_BACKGROUND = Color.Red;
        public static readonly Color ALARM_COLOR_TEXT = Color.Black;
        public static readonly Color ALARM_COLOR_BACKGROUND = Color.FromArgb(190, 0, 0);
        public static readonly Color NORMAL_NOT_ACKNOWLEDGED_COLOR_TEXT = Color.Black;
        public static readonly Color NORMAL_NOT_ACKNOWLEDGED_COLOR_BACKGROUND = Color.LimeGreen;
        public static readonly Color NORMAL_COLOR_TEXT = Color.Black;
        public static readonly Color NORMAL_COLOR_BACKGROUND = Color.SkyBlue;
        public static readonly Color NO_ALARMS_IN_QUEUE_COLOR_TEXT = Color.Black;
        public static readonly Color NO_ALARMS_IN_QUEUE_COLOR_BACKGROUND = Color.LightGray;

        public const string PLUGINS_DIR = "plugins";

        public const string NOVA_SERVER = "Contal Nova Server";

        public const string REGISTRY_CONNECTION_STRING = @"HKLM\Software\Contal\Cgp\Server\Database";
        public const string REGISTRY_CONNECTION_STRING_EXTERN_DATABASE = @"HKLM\Software\Contal\Cgp\Server\ExternDatabase";

        public const int DEFAULT_REMOTING_SERVER_PORT = 54001;
        public const int DEFAULT_REMOTING_CLIENT_PORT = 54002;
        public const int DEFAULT_REMOTING_CLIENT_PORT_UPPER = 54009;
        public const int DEFAULT_SMTP_PORT = 25;
        public const string DEFAULT_LANGUAGE = "English";

        public const string REGISTRY_GENERAL_SETTINGS = @"HKLM\Software\Contal\Cgp\Server\General";
        public const string REGISTRY_GENERAL_SETTINGS_WOW6432NODE = @"HKLM\Software\Wow6432Node\Contal\Cgp\Server\General";
        public const string TIMESTAMP_VALUE_NAME = "TimeStamp";

        public const string CGP_REMOTING_SERVER_PORT = "RemotingServerPort";
        public const string CGP_REMOTING_SERVER_IP_ADDRESS = "RemotingServerIpAddress";
        public const string CGP_SERVER_FRIENDLY_NAME = "FriendlyName";

        public const string CGP_AES_REMOTING_ENABLED = "AesRemotingEnabled";
        public const string CGP_AES_REMOTING_KEY = "AesRemotingKey";
        public const string CGP_CNMP_UDP_PORT = "CnmpUdpPort";
        public const string CGP_SERVER_LICENCE_PATH = "LicencePath";
        public const string CGP_DHCP_AUTOSTART = "AutoStartDHCP";
        public const string CGP_ENABLE_PARENT_IN_FULL_NAME = "EnableParentInFullName";
        public const string CGP_SHOW_HIDDEN_FEATURES = "ShowHiddenFeatures";
        public const string DELETE_EVENTS_FEATURE_NAME = "DeleteEvents";
        public const string EXTENDED_TIMEBUYING_FEATURE_NAME = "ExtendedTimeBuying";

        public const string WINDOWS_DWM_SETTINGS = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM";
        public const string DWM_COLORIZATION_COLOR_VALUE = "ColorizationColor";
        public const string DWM_PREVALENCE_VALUE = "ColorPrevalence";

        /// <summary>
        /// constants used as CNMP Extra identifiers for Client-Server lookup
        /// </summary>
        public const string CNMP_SERVER_PORT = "ServerPort";
        public const string CNMP_SERVER_MACHINE_NAME = "MachineName";
        public const string CNMP_SERVER_MULTIHOMING = "ServerMultihoming";
        public const string CNMP_SERVER_VERSION = "ServerVersion";
        public const string CNMP_SERVER_EDITION_NAME = "ServerEditionName";

        public const string CGP_SERVER_CURRENT_LANGUAGE = "CurrentLanguage";

        public readonly byte[] REMOTING_KEY = { 0x65, 0x37, 0x6D, 0xA3, 0x03, 0x8E, 0x03, 0x89, 0xB6, 0xD9, 0x28, 0x42, 0xC1, 0xFB, 0x5F, 0x54, 0x0F,
                                                         0x66, 0x7A, 0x70, 0x8E, 0xB2, 0xAE, 0x4F, 0x49, 0x5E, 0x97, 0x2B, 0xF3, 0x47, 0x60, 0xF8};
        public readonly byte[] REMOTING_SALT = { 0xCA, 0xB6, 0x8D, 0xF5, 0xE3, 0x48, 0x7F, 0xAB, 0xDB, 0x63, 0x7C, 0x12, 0x9F, 0x61, 0x13, 0x26 };

        /// <summary>
        /// STANDARDIZED SPEC FOR CGP UNDER CNMP 2.0 PROTOCOL
        /// </summary>
        public const string CNMP_INSTANCE_PREFIX = "/Cgp/Server/";
        public const string CNMP_TYPE = "/Cgp/Server";


        public const string CGP_SERVICE_NAME = "ContalNovaServerService";
        public const string CGP_CONSOLE_NAME = "ContalNovaServerConsole";

        public const string CGP_SERVICE_USER = "NovaServiceUser";

        public const string DEFAULT_ADMIN_LOGIN = "admin";
        public const string DEFAULT_ADMIN_LOGIN_GROUP = "admins";

        public const string CGP_SMTP_SERVER = "SmtpServer";
        public const string CGP_SMTP_PORT = "SmtpPort";
        public const string CGP_SMTP_SOURCE_EMAIL_ADDRESS = "SmtpSourceEmailAddress";
        public const string CGP_SMTP_SUBJECT = "SmtpSubject";
        public const string CGP_SMTP_CREDENTIALS = "SmtpCredentials";
        public const string CGP_SMTP_SSL = "SmtpSsl";

        public const string CGP_SERIAL_PORT = "SerialPort";
        public const string CGP_SERIAL_PORT_BAUD_RATE = "SerialPortBaudRate";
        public const string CGP_SERIAL_PORT_DATA_BITS = "SerialPortDataBits";
        public const string CGP_SERIAL_PORT_PARITY = "SerialPortParity";
        public const string CGP_SERIAL_PORT_STOP_BITS = "SerialPortStopBits";
        public const string CGP_SERIAL_PORT_FLOW_CONTROL = "SerialPortFlowControl";
        public const string CGP_SERIAL_PORT_PARITY_CHECK = "SerialPortParityCheck";
        public const string CGP_SERIAL_PORT_CARRIER_DETECT = "SerialPortCarrierDetect";
        public const string CGP_SERIAL_PORT_PIN = "SerialPortPin";

        public const string CGP_DATABASE_BACKUP_PATH = "DatabaseBackupPath";
        public const string CGP_DATABASE_BACKUP_TIME_ZONE = "DatabaseBackupTimeZone";
        public const string CGP_EVENTLOGS_EXPIRATION_DAYS = "EventlogsExpirationDays";
        public const string CGP_EVENTLOGS_MAX_COUNT_VALUES = "EventlogsMaxCountValue";
        public const string CGP_EVENTLOGS_MAX_COUNT_EXPONENT = "EventlogsMaxCountExponent";
        public const string CGP_EVENTLOGS_EXPIRATION_TIME_ZONE = "EventlogsExpirationTimeZone";

        public const string CGP_DRAG_DROP_COLOR_TEXT = "CgpDragDropColorText";
        public const string CGP_DRAG_DROP_COLOR_BACKGROUND = "CgpDragDropColorBackground";
        public const string CGP_REFERENCE_OBJECT_COLOR_TEXT = "CgpReferenceObjectColorText";
        public const string CGP_REFERENCE_OBJECT_COLOR_BACKGROUND = "CgpReferenceObjectColorBackground";
        public const string CGP_ALARM_NOT_ACKNOWLEDGED_COLOR_TEXT = "CgpAlarmNotAcknowledgedColorText";
        public const string CGP_ALARM_NOT_ACKNOWLEDGED_COLOR_BACKGROUND = "CgpAlarmNotAcknowledgedColorBackground";
        public const string CGP_ALARM_COLOR_TEXT = "CgpAlarmColorText";
        public const string CGP_ALARM_COLOR_BACKGROUND = "CgpAlarmColorBackground";
        public const string CGP_NORMAL_NOT_ACKNOWLEDGED_COLOR_TEXT = "CgpNormalNotAcknowledgedColorText";
        public const string CGP_NORMAL_NOT_ACKNOWLEDGED_COLOR_BACKGROUND = "CgpNormalNotAcknowledgedColorBackground";
        public const string CGP_NORMAL_COLOR_TEXT = "CgpNormalColorText";
        public const string CGP_NORMAL_COLOR_BACKGROUND = "CgpNormalColorBackground";
        public const string CGP_NO_ALARMS_IN_QUEUE_COLOR_TEXT = "CgpNoAlarmsInQueueColorText";
        public const string CGP_NO_ALARMS_IN_QUEUE_COLOR_BACKGROUND = "CgpNoAlarmsInQueueColorBackground";

        public const string CGP_AUTOCLOSE_TURNED_ON = "CgpAutoCloseTurnedOn";
        public const string CGP_AUTOCLOSE_TIMEOUT = "CgpAutoCloseTimeout";

        public const string CGP_CHANGE_PASSWORD_DAYS = "CgpChangePasswordDay";
        public const string CGP_REQUIRE_PIN_CARD_LOGIN = "CgpRequirePINCardLogin";
        public const string CGP_UNIQUE_NOT_NULL_PERSONAL_KEY = "CgpUniqueNotNullPersonalKey";
        public const string CGP_LOCK_CLIENT_APPLICATION = "CgpLockClientApplication";
        public const string CGP_CCU_CONFIGURATION_TO_SERVER_BY_PASSWORD = "CcuConfigurationToServerByPassword";
        public const string CGP_REQUIRED_SECURE_PIN = "CgpRequredSecurePin";
        public const string CGP_DISABLE_CCU_PLUG_N_PLAY_AUTOMATIC_ASSIGNMENT = "DisableCcuPnPAssigmnet";
        public const string CGP_LIST_ONLY_UNASSIGNED_CARDS_IN_PERSON_FORM = "ListOnlyUnassignedCardsInPersonForm";
        public const string CGP_DELAY_TO_SAVE_ALARMS_FROM_CARD_READERS = "DelayToSaveAlarmsFromCardReaders";

        public const string CGP_EVENTLOG_INPUT_STATE_CHANGED = "EventlogInputStateChanged";
        public const string CGP_EVENTLOG_OUTPUT_STATE_CHANGED = "EventlogOutputStateChanged";
        public const string CGP_EVENTLOG_ALRM_AREA_ALARM_STATE_CHANGED = "EventlogAlarmAreaAlarmStateChanged";
        public const string CGP_EVENTLOG_ALRM_AREA_ACTIVATION_STATE_CHANGED = "EventlogAlarmAreaActivationStateChanged";
        public const string CGP_EVENTLOG_CARD_READER_ONLINE_STATE_CHANGED = "EventlogCardReaderOnlineStateChanged";

        public const string CGP_ADVANCED_DEVICE_SETTINGS_MAX_EVENTS_COUNT_SD_CARD = "AdvancedDeviceSettingsMaxEventsCountSDCard";
        public const string CGP_ADVANCED_DEVICE_SETTINGS_MAX_EVENTS_COUNT_NAND_FLASH = "AdvancedDeviceSettingsMaxEventsCountNandFlash";
        public const string CGP_ADVANCED_DEVICE_SETTINGS_NO_FLASHING_EVENTS = "AdvancedDeviceSettingsNoFlashingEvents";
        public const string CGP_ADVANCED_DEVICE_SETTINGS_SYNCING_TIME_FROM_SERVER = "AdvancedDeviceSettingsSyncingTimeFromServer";
        public const string CGP_ADVANCED_DEVICE_SETTINGS_PERIOD_OF_TIME_SYNC_WITHOUT_STRATUM = "AdvancedDeviceSettingsPeriodOfTimeSyncWithoutStratum";
        public const string CGP_ADVANCED_DEVICE_SETTINGS_PERIODIC_TIME_SYNC_TOLERANCE = "AdvancedDeviceSettingsPeriodicTimeSyncTolerance";

        public const string CGP_ADVANCED_SETTINGS_MAX_EVENTS_COUNT_FOR_INSERT = "AdvancedSettingsMaxEventsCountForInsert";
        public const string CGP_ADVANCED_SETTINGS_DELAY_FOR_SAVE_EVENTS = "AdvancedSettingsDelayForSaveEvents";

        public const int DEFAULT_DELAY_TO_SAVE_ALARMS_FROM_CARD_READERS = 24;
        public const int DEFAULT_MAX_EVENTS_COUNT_FOR_INSERT = 100;
        public const int DEFAULT_DELAY_FOR_SAVE_EVENTS = 500;
        public const int DEFAULT_PERIOD_OF_TIME_SYNC_WITHOUT_STRATUM = 2;
        public const int DEFAULT_PERIODIC_TIME_SYNC_TOLERANCE = 5;
        public const int DEFAULT_CLIENT_SESION_TIMEOUT = 5;
        public const int DEFAULT_ALARM_LIST_SUSPEND_REFRESH_TIMEOUT = 3;
        public const int DEFAULT_QUICK_DB_EVENTLOG_VIEW_SPAN = 1;
        public const int DEFAULT_DELAY_FOR_SENDING_CHANGES_TO_CCU = 5;

        public static string[] SERVICE_DEPENDENCIES = { "winmgmt", "tcpip", "Netman" };

        public const string SID_ADMINISTRATORS = "S-1-5-32-544";

        public static readonly byte[] DATABASE_KEY = { 0x5D, 0xE5, 0x5D, 0x68, 0x2C, 0x46, 0x57, 0x9C, 0x72, 0x5B, 0x3B, 0xC6, 0x34, 0xCB, 0x37, 0x16, 0xC9, 0x8A, 0xB3, 0x69, 0x90, 0xC3, 0xEC, 0x76, 0x89, 0x44, 0x01, 0x33, 0x19, 0x35, 0x00, 0x43 };
        public static readonly byte[] DATABASE_SALT = { 0xCC, 0xC8, 0x62, 0x69, 0x28, 0x2F, 0x85, 0x22, 0xAD, 0x91, 0x56, 0x3B, 0xEB, 0xB5, 0xF9, 0xEC };
    }
}
