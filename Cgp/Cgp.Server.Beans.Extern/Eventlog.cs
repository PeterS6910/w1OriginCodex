using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans.Extern
{
    [Serializable]
    public class Eventlog : AOrmObject
    {
        public const string TYPECONNECTDATABASE = "Connect database";
        public const string TYPEDISCONNECTDATABASE = "Disconnect database";
        public const string TYPEDATABASEBACKUP = "Database backup";
        public const string TYPECLIENTCONNECT = "Connect client";
        public const string TYPECLIENTDISCONNECT = "Disconnect client";
        public const string TYPECLIENTLOGIN = "Client login";
        public const string TYPECLIENTLOGOUT = "Client logout";
        public const string TYPECLIENTLOGINWRONGPASSWORD = "Client login wrong password";
        public const string TYPEPRESENTATIONPROCESSOR = "Presentation processor error";
        public const string TYPEINITIALIZEPLUGIN = "Initialize plugin";
        public const string TYPELOADPLUGINSUCCESFUL = "Load plugin succesful";
        public const string TYPEALARMOCCURED = "Alarm occured";
        public const string TYPEALARMAREAACTIVATIONSTATECHANGED = "Alarm area activation state changed";
        public const string TYPEACTALARMACKNOWLEDGED = "Active alarm acknowledged";
        public const string TYPEINACTALARMACKNOWLEDGED = "Inactive alarm acknowledged and removed";
        public const string TYPEBINDINGEVENTFAILED = "Binding event failed";
        public const string TYPERUNMETHODFAILED = "Run method failed";
        public const string TYPE_CCU_EXCEPTION_OCCURRED = "CCU exception occurred";
        public const string TYPEACCESSDENIED = "Access denied";
        public const string TYPEACCESSDENIEDINVALIDPIN = "Access denied invalid PIN";
        public const string TYPEACCESSDENIEDINVALIDCODE = "Access denied invalid CODE";
        public const string TYPEACCESSDENIEDINVALIDEMERGENCYCODE = "Access denied invalid emergency code";
        public const string TYPEUNKNOWNCARD = "Unknown card";
        public const string TYPEACCESSDENIEDCARDBLOCKEDORINACTIVE = "Access denied card blocked or inactive";
        public const string TYPEACCESSDENIEDSETALARMAREAINVALIDCODE = "Access denied set alarm area invalid CODE";
        public const string TYPEACCESSDENIEDUNSETALARMAREAINVALIDCODE = "Access denied unset alarm area invalid CODE";
        public const string TYPEACCESSDENIEDSETALARMAREAINVALIDPIN = "Access denied set alarm area invalid PIN";
        public const string TYPEACCESSDENIEDUNSETALARMAREAINVALIDPIN = "Access denied unset alarm area invalid PIN";
        public const string TYPEACCESSDENIEDSETALARMAREANORIGHTS = "Access denied set alarm area no rights";
        public const string TYPEACCESSDENIEDUNSETALARMAREANORIGHTS = "Access denied unset alarm area no rights";
        public const string TYPESETALARMAREAFROMCARDREADER = "Set alarm area from card reader";
        public const string TYPEUNSETALARMAREAFROMCARDREADER = "Unset alarm area from card reader";
        public const string TYPESETALARMAREABYOBJECTFORAA = "Set alarm area by object for automatic activation";
        public const string TYPEUNSETALARMAREABYOBJECTFORAA = "Unset alarm area by object for automatic activation";
        public const string TYPEDSMACCESSPERMITTED = "DSM access permitted";
        public const string TYPEDSMNORMALACCESS = "DSM normal access";
        public const string TYPEDSMAPASRESTORED = "DSM APAS restored";
        public const string TYPEDSMACCESSINTERRUPTED = "DSM access interrupted";
        public const string TYPEDSMACCESSRESTRICTED = "DSM access restricted";
        public const string TYPEDSMACCESSVIOLATED = "DSM access violated";
        public const string TYPEDCUONLINE = "DCU online";
        public const string TYPEDCUOFFLINE = "DCU offline";
        public const string TYPEDCUUPGRADING = "DCU upgrading";
        public const string TYPEDCUSUPGRADEFAILED = "DCUs upgrade failed";
        public const string TYPECEUPGRADEFAILED = "CE upgrade failed";
        public const string TYPECCUUPGRADEFAILED = "CCU upgrade failed";
        public const string TYPECARDREADERSUPGRADEFAILED = "Card readers upgrade failed";
        public const string TYPEDCUWAITINGFORUPGRADE = "DCU waiting for upgrade";
        public const string TYPEDCUAUTOUPGRADING = "DCU auto upgrading";
        public const string TYPECREATEDNEWDCU = "Created new DCU";
        public const string TYPEDCUCOMMANDTIMEOUT = "DCU command time out";
        public const string TYPEDCUCOMMANDNACK = "DCU command not ack";
        public const string TYPEDCUNODASSIGNED = "DCU node assigned";
        public const string TYPEDCUNODRENEWED = "DCU node renewed";
        public const string TYPEDCUNODRELEASED = "DCU node released";
        public const string TYPEFAILEDTOUPDATESTRUCTUREONCCU = "Failed to update structure on CCU";
        public const string TYPE_INPUT_STATE_CHANGED = "Input state changed";
        public const string TYPE_OUTPUT_STATE_CHANGED = "Output state changed";
        public const string TYPE_ALARM_AREA_ALARM_STATE_CHANGED = "Alarm area alarm state changed";
        public const string TYPE_CARD_READER_ONLINE_STATE_CHANGED = "Card reader online state changed";
        public const string TYPE_CARDSYSTEM_REMOVED = "Card system removed";
        public const string TYPE_CARDSYSTEM_ADDED = "Card system added";
        public const string TYPE_INITIAL_OBJECT_SENDING_FAILED = "Initial object sending failed";
        public const string TYPE_CARD_ENCODED = "Card encoded";
        public const string TYPE_ACTUAL_STATE_OF_SECURITY_DAILY_PLAN = "Actual state of security daily plan";
        public const string TYPE_ACTUAL_STATE_OF_SECURITY_TIME_ZONE = "Actual state of security time zone";
        public const string TYPE_TIME_FOR_NEXT_EVALUATING_STATES_OF_SDP = "Time for next evaluating states of security daily plans";
        public const string TYPE_COPROCESSOR_FAILURE_CHANGED = "Coprocessor failure changed";
        public const string TYPE_CCU_TIME_ADJUSTED = "CCU time adjusted";
        public const string TYPE_CCU_OBJECT_DESERIALIZE_FAILED = "CCU object deserialize failed";
        public const string TYPE_CCU_TIMING_PROBLEM = "CCU timing problem";
        public const string TYPE_DSM_STATE_CHANGED = "DSM state changed";
        public const string TYPE_CCU_ALARM_PRIMARY_POWER_MISSING = "CCU alarm primary power missing";
        public const string TYPE_CCU_ALARM_BATTERY_IS_LOW = "CCU alarm battery is low";
        public const string TYPE_CCU_ALARM_FUSE_ON_EXTENSION_BOARD = "CCU alarm fuse on extension board";
        public const string TYPE_CCU_TAMPER_STATE_CHANGED = "CCU tamper state changed";
        public const string TYPE_DCU_TAMPER_STATE_CHANGED = "DCU tamper state changed";
        public const string TYPE_CARD_READER_TAMPER_STATE_CHANGED = "Card reader tamper state changed";
        public const string TYPE_CCU_GETFROMDATABASE_RETURN_NULL = "CCU returned null in loading the object";
        public const string TYPE_CCU_DATA_CHANNEL_TRANSFER_FAILED = "CCU data channel transfer failed";
        public const string TYPE_CCU_INCOMING_TRANSFER_INFO = "CCU incoming transfer info";
        public const string TYPE_ALARM_AREA_SETUNSET_NOT_RESPOND = "EIS did not respond in proper time";
        public const string TYPE_ICCU_SENDING_OF_OBJECT_STATE_FAILED = "ICCU sending of object state failed";
        public const string TYPE_ICCU_PORT_ALREADY_USED = "ICCU port is already used";
        public const string TYPE_ALARM_AREA_SET_FROM_CR_FAILED = "Set alarm area from card reader failed";
        public const string TYPE_CCU_INCOMING_AUTONOMOUS_EVENTS = "CCU incoming autonomous events";
        public const string TYPE_CCU_AUTONOMOUS_EVENTS_PROCESSING_CANCELED = "CCU autonomous events processing canceled";
        public const string TYPE_CCU_AUTONOMOUS_EVENTS_PROCESSED = "CCU autonomous events processed";
        public const string TYPE_CCU_ONLINE = "CCU online";
        public const string TYPE_CCU_OFFLINE = "CCU offline";
        public const string TYPE_CCU_MEMORY_LOAD = "CCU memory load";
        public const string TYPE_CCU_FILESYSTEM_PROBLEM = "CCU filesystem problem";
        public const string TYPE_CCU_PROCESSING_STREAM_TAKES_TOO_LONG_TIME = "CCU processing of the stream takes too long time";
        public const string TYPE_CCU_SD_CARD_NOT_FOUND = "CCU SD card not found";
        public const string TYPE_ALARM_AREA_BOUGHT_TIME_CHANGED = "Alarm area bought time changed";
        public const string TYPE_ALARM_AREA_BOUGHT_TIME_EXPIRED = "Alarm area bought time expired";
        public const string TYPE_ALARM_AREA_TIME_BUYING_FAILED = "Alarm area time buying failed";
        public const string TYPE_ANTI_PASS_BACK_ZONE_CARD_ENTERED = "Anti-passback zone entry";
        public const string TYPE_ANTI_PASS_BACK_ZONE_CARD_EXITED = "Anti-passback zone exit";
        public const string TYPE_ANTI_PASS_BACK_ZONE_CARD_TIMED_OUT = "Anti-passback zone time-out";
        public const string TYPE_FUNCTIONKEY_PRESSED = "Function key pressed";
        public const string TYPE_SET_ALARM_AREA_FROM_CLIENT = "Set alarm area from client";
        public const string TYPE_UNSET_ALARM_AREA_FROM_CLIENT = "Unset alarm area from client";
        public const string TYPE_CCU_ALARM_UPS_OUTPUT_FUSE = "CCU alarm UPS output fuse";
        public const string TYPE_CCU_ALARM_UPS_BATTERY_FAULT = "CCU alarm UPS battery fault";
        public const string TYPE_CCU_ALARM_UPS_BATTERY_FUSE = "CCU alarm UPS battery fuse";
        public const string TYPE_CCU_ALARM_UPS_OVERTEMPERATURE = "CCU alarm UPS overtemperature";
        public const string TYPE_CCU_ALARM_UPS_TAMPER = "CCU alarm UPS tamper";
        public const string TYPE_ACCESS_DENIED_ENTER_TO_AA_MENU_INVALID_PIN = "Access denied enter to AA menu invalid PIN";
        public const string TYPE_ACCESS_DENIED_ENTER_TO_AA_MENU_INVALID_CODE = "Access denied enter to AA menu invalid CODE";
        public const string TYPE_ACCESS_DENIED_ENTER_TO_SENSORS_MENU_INVALID_PIN = "Access denied enter to sensors menu invalid PIN";
        public const string TYPE_ACCESS_DENIED_ENTER_TO_SENSORS_MENU_INVALID_CODE = "Access denied enter to sensors menu invalid CODE";
        public const string TYPE_ACCESS_DENIED_ENTER_TO_EVENTLOGS_MENU_INVALID_PIN = "Access denied enter to eventlogs menu invalid PIN";
        public const string TYPE_ACCESS_DENIED_ENTER_TO_EVENTLOGS_MENU_INVALID_CODE = "Access denied enter to eventlogs menu invalid CODE";
        public const string TYPE_TIMETEC_COMMUNICATION_ONLINE_STATE_CHANGED = "Timetec online state changed";
        public const string TYPE_TIMETEC_EVENT_TRANSFER_FAILED = "Timetec event transfer failed";
        public const string TYPE_TIMETEC_EVENT_SAVE_OBJECT = "Timetec event save object";
        public const string TYPE_TIMETEC_EVENT_DELETE_OBJECT = "Timetec event delete object";
        public const string TYPE_CCU_CE_VERSIONS_WERE_LOADED = "CCU and CE versions were loaded";

        public const string COLUMN_ID_EVENTLOG = "IdEventlog";
        public const string COLUMN_EVENTLOG_DATE_TIME = "EventlogDateTime";
        public const string COLUMN_CGPSOURCE = "CGPSource";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string COLUMN_TYPE = "Type";
        public const string COLUMN_EVENTSOURCES = "EventSources";
        public const string COLUMN_EVENTLOG_PARAMETERS = "EventlogParameters";
 
        public Eventlog(string type, DateTime eventlogDateTime, string cgpSource, string description)
        {
            IwQuick.Validator.CheckNullString(type);
            IwQuick.Validator.CheckNullString(cgpSource);
            IwQuick.Validator.CheckNullString(description);

            Type = type;
            EventlogDateTime = eventlogDateTime;
            CGPSource = cgpSource;
            //Localization = localization;
            Description = description;
            //EventSourceGuid = eventSourceGuid;
            //EventSourceObjectType = (byte)eventSourceType;
        }

        public Eventlog()
        {
        }

        public long IdEventlog { get; set; }
        public string Type { get; set; }
        public DateTime EventlogDateTime { get; set; }
        public string CGPSource { get; set; }
        public IList<EventSource> EventSources { get; set; }
        public string Description { get; set; }
        public IList<EventlogParameter> EventlogParameters { get; set; }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            return obj is Eventlog && (obj as Eventlog).IdEventlog == IdEventlog;
        }

        public override string ToString()
        {
            return EventlogDateTime.ToString("dd.MM.yyyy HH:mm:ss") + ": " + Type;
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();

            if (ToString().ToLower().Contains(expression)) 
                return true;

            return Description.ToLower().Contains(expression);
        }

        public override string GetIdString()
        {
            return IdEventlog.ToString();
        }

        public override object GetId()
        {
            return IdEventlog;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override Globals.ObjectType GetObjectType()
        {
            return Globals.ObjectType.Eventlog;
        }
    }
}
