using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovaEventLogs
{
    public class ElsC
    {
        private List<string> _eventTypes = new List<string>();
        private void FillEventTypes()
        {
            _eventTypes.Clear();
            _eventTypes.Add(Eventlog.TYPECONNECTDATABASE);
            _eventTypes.Add(Eventlog.TYPEDISCONNECTDATABASE);
            _eventTypes.Add(Eventlog.TYPECLIENTCONNECT);
            _eventTypes.Add(Eventlog.TYPECLIENTDISCONNECT);
            _eventTypes.Add(Eventlog.TYPECLIENTLOGIN);
            _eventTypes.Add(Eventlog.TYPECLIENTLOGOUT);
            _eventTypes.Add(Eventlog.TYPECLIENTLOGINWRONGPASSWORD);
            _eventTypes.Add(Eventlog.TYPEPRESENTATIONPROCESSOR);
            _eventTypes.Add(Eventlog.TYPEINITIALIZEPLUGIN);
            _eventTypes.Add(Eventlog.TYPELOADPLUGINSUCCESFUL);
            _eventTypes.Add(Eventlog.TYPEALARMOCCURED);
            _eventTypes.Add(Eventlog.TYPEALARMAREAACTIVATIONSTATECHANGED);
            _eventTypes.Add(Eventlog.TYPEACTALARMACKNOWLEDGED);
            _eventTypes.Add(Eventlog.TYPEINACTALARMACKNOWLEDGED);
            _eventTypes.Add(Eventlog.TYPEBINDINGEVENTFAILED);
            _eventTypes.Add(Eventlog.TYPERUNMETHODFAILED);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIED);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDINVALIDPIN);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDINVALIDGIN);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDINVALIDEMERGENCYCODE);
            _eventTypes.Add(Eventlog.TYPEUNKNOWNCARD);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDCARDBLOCKEDORINACTIVE);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDSETALARMAREAINVALIDGIN);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDUNSETALARMAREAINVALIDGIN);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDSETALARMAREAINVALIDPIN);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDUNSETALARMAREAINVALIDPIN);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDSETALARMAREANORIGHTS);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDUNSETALARMAREANORIGHTS);
            _eventTypes.Add(Eventlog.TYPESETALARMAREAFROMCARDREADER);
            _eventTypes.Add(Eventlog.TYPEUNSETALARMAREAFROMCARDREADER);
            _eventTypes.Add(Eventlog.TYPESETALARMAREABYOBJECTFORAA);
            _eventTypes.Add(Eventlog.TYPEUNSETALARMAREABYOBJECTFORAA);
            _eventTypes.Add(Eventlog.TYPEDSMACCESSPERMITTED);
            _eventTypes.Add(Eventlog.TYPEDSMAPASRESTORED);
            _eventTypes.Add(Eventlog.TYPEDSMNORMALACCESS);
            _eventTypes.Add(Eventlog.TYPEDSMACCESSINTERRUPTED);
            _eventTypes.Add(Eventlog.TYPEDSMACCESSRESTRICTED);
            _eventTypes.Add(Eventlog.TYPEDSMACCESSVIOLATED);
            _eventTypes.Add(Eventlog.TYPEDCUONLINE);
            _eventTypes.Add(Eventlog.TYPEDCUOFFLINE);
            _eventTypes.Add(Eventlog.TYPEDCUUPGRADING);
            _eventTypes.Add(Eventlog.TYPEDCUSUPGRADEFAILED);
            _eventTypes.Add(Eventlog.TYPECARDREADERSUPGRADEFAILED);
            _eventTypes.Add(Eventlog.TYPEDCUWAITINGFORUPGRADE);
            _eventTypes.Add(Eventlog.TYPEDCUAUTOUPGRADING);
            _eventTypes.Add(Eventlog.TYPECREATEDNEWDCU);
            _eventTypes.Add(Eventlog.TYPEDCUCOMMANDTIMEOUT);
            _eventTypes.Add(Eventlog.TYPEDCUCOMMANDNACK);
            _eventTypes.Add(Eventlog.TYPEDCUNODASSIGNED);
            _eventTypes.Add(Eventlog.TYPEDCUNODRENEWED);
            _eventTypes.Add(Eventlog.TYPEDCUNODRELEASED);
            _eventTypes.Add(Eventlog.TYPEFAILEDTOUPDATESTRUCTUREONCCU);
            _eventTypes.Add(Eventlog.TYPE_CARDSYSTEM_ADDED);
            _eventTypes.Add(Eventlog.TYPE_CARDSYSTEM_REMOVED);
        }

        public List<string> GetEventLogTypes()
        {
            FillEventTypes();
            return _eventTypes;
        }
    }

    public class Eventlog
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
        public const string TYPEACCESSDENIED = "Access denied";
        public const string TYPEACCESSDENIEDINVALIDPIN = "Access denied invalid PIN";
        public const string TYPEACCESSDENIEDINVALIDGIN = "Access denied invalid GIN";
        public const string TYPEACCESSDENIEDINVALIDEMERGENCYCODE = "Access denied invalid emergency code";
        public const string TYPEUNKNOWNCARD = "Unknown card";
        public const string TYPEACCESSDENIEDCARDBLOCKEDORINACTIVE = "Access denied card blocked or inactive";
        public const string TYPEACCESSDENIEDSETALARMAREAINVALIDGIN = "Access denied set alarm area invalid GIN";
        public const string TYPEACCESSDENIEDUNSETALARMAREAINVALIDGIN = "Access denied unset alarm area invalid GIN";
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
    }
}
