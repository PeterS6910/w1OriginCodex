using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.IwQuick;
using Contal.Cgp.NCAS.Globals;

namespace Contal.Cgp.NCAS.Server
{
    public class EventLogBuilder
    {
        private readonly string _thisAssemblyName;

        private readonly string _ccuIpAddress;
        private readonly DateTime _dateTime;
        private readonly EventType _eventType;
        private readonly Guid _objectGuid;
        private readonly object[] _parameters;

        private Eventlog _outEventlog;
        private List<EventSource> _outEventSources;
        private List<EventlogParameter> _outEventlogParameters;

        public Eventlog OutEventlog
        {
            get { return _outEventlog; }
        }

        public List<EventSource> OutEventSources
        {
            get { return _outEventSources; }
        }

        public List<EventlogParameter> OutEventlogParameters
        {
            get { return _outEventlogParameters; }
        }

        public EventLogBuilder(
            string assemblyName,
            string ccuIpAddress,
            EventOptions eventOptions)
        {
            _thisAssemblyName = assemblyName;

            _ccuIpAddress = ccuIpAddress;
            _dateTime = eventOptions.DateTime;
            _eventType = eventOptions.EventType;
            _objectGuid = eventOptions.ObjectGuid;
            _parameters = eventOptions.Parameters;
        }

        public bool Build()
        {
            switch (_eventType)
            {
                case EventType.InputStateChanged:
                    return HandleInputStateChanged();

                case EventType.OutputStateChanged:
                    return HandleOutputStateChanged();

                case EventType.AlarmAreaAlarmStateChanged:
                    return HandleAlarmAreaStateChanged();

                case EventType.AlarmAreaActivationStateChanged:
                    return HandleAlarmAreaActivationStateChanged();

                case EventType.UnknownCard:
                    return HandleUnknownCard();

                case EventType.AccessDenied:
                    return HandleAccessDenied();

                case EventType.AccessDeniedInvalidPin:
                    return HandleAccessDeniedInvalidPin();

                case EventType.AccessDeniedInvalidGin:
                    return HandleAccessDeniedInvalidGin();

                case EventType.AccessDeniedInvalidEmergencyCode:
                    return HandleAccessDeniedInvalidEmergencyCode();

                case EventType.AccessDeniedCardBlockedOrInactive:
                    return HandleAccessDeniedCardBlockedOrInactive();

                case EventType.AccessDeniedSetAlarmAreaInvalidPin:
                    return HandleAccessDeniedSetAlarmAreaInvalidPin();

                case EventType.AccessDeniedUnsetAlarmAreaInvalidPin:
                    return HandleAccessDeniedUnsetAlarmAreaInvalidPin();

                case EventType.AccessDeniedSetAlarmAreaNoRights:
                    return HandleAccessDeniedSetAlarmAreaNoRights();

                case EventType.AccessDeniedUnsetAlarmAreaNoRights:
                    return HandleAccessDeniedUnsetAlarmAreaNoRights();

                case EventType.AlarmAreaSetFromCRFailed:
                    return HandleAlarmAreaSetFromCRFailed();

                case EventType.AccessDeniedSetAlarmAreaInvalidGin:
                    return HandleAccessDeniedSetAlarmAreaInvalidGin();

                case EventType.AccessDeniedUnsetAlarmAreaInvalidGin:
                    return HandleAccessDeniedUnsetAlarmAreaInvalidGin();

                case EventType.SetAlarmAreaByObjectForAutomaticActivation:
                    return HandleSetAlarmAreaByObjectForAutomaticActivation();

                case EventType.UnsetAlarmAreaByObjectForAutomaticActivation:
                    return HandleUnsetAlarmAreaByObjectForAutomaticActivation();

                case EventType.SetAlarmAreaFromCardReader:
                    return HandleSetAlarmAreaFromCardReader();

                case EventType.UnsetAlarmAreaFromCardReader:
                    return HandleUnsetAlarmAreaFromCardReader();

                case EventType.DSMAccessPermitted:
                    return HandleDSMAccessPermitted();

                case EventType.DSMNormalAccess:
                    return HandleDSMNormalAccess();

                case EventType.DSMAccessInterupted:
                    return HandleDSMAccessInterupted();

                case EventType.DSMAccessRestricted:
                    return HandleDSMAccessRestricted();

                case EventType.DSMAccessViolated:
                    return HandleDSMAccessViolated();

                case EventType.DSMApasRestored:
                    return HandleDSMApasRestored();

                case EventType.DcuCommandTimeOut:
                    return HandleDcuCommandTimeOut();

                case EventType.DcuCommandNotACK:
                    return HandleDcuCommandNotACK();

                case EventType.DcuNodeAssigned:
                    return HandleDcuNodeAssigned();

                case EventType.DcuNodeRenewed:
                    return HandleDcuNodeRenewed();

                case EventType.DcuNodeReleased:
                    return HandleDcuNodeReleased();

                case EventType.SectorCardSystemAdded:
                    return HandleSectorCardSystemAdded();

                case EventType.SectorCardSystemRemoved:
                    return HandleSectorCardSystemRemoved();

                case EventType.RunMethodFailed:
                    return HandleRunMethodFailed();

                case EventType.SecurityTimeChannelChanged:
                    return HandleSecurityTimeChannelChanged();

                case EventType.CoprocessorFailureChanged:
                    return HandleCoprocessorFailureChanged();

                case EventType.CCUTimeAdjusted:
                    return HandleCCUTimeAdjusted();

                case EventType.ObjectDeserializeFailed:
                    return HandleObjectDeserializeFailed();

                case EventType.CCUTimingProblem:
                    return HandleCCUTimingProblem();

                case EventType.DSMStateChanged:
                    return HandleDSMStateChanged();

                case EventType.AlarmCCUPrimaryPowerMissing:
                    return HandleAlarmCCUPrimaryPowerMissing();

                case EventType.AlarmCCUBatteryIsLow:
                    return HandleAlarmCCUBatteryIsLow();

                case EventType.AlarmCCUExtFuse:
                    return HandleAlarmCCUExtFuse();

                case EventType.SendTamper:
                    return HandleSendTamper();

                case EventType.GetFromDatabaseReturnNull:
                    return HandleGetFromDatabaseReturnNull();

                case EventType.CCUIncomingTransferInfo:
                    return HandleCCUIncomingTransferInfo();

                case EventType.NotConfirmSetUnsetAAFromEIS:
                    return HandleNotConfirmSetUnsetAAFromEIS();

                case EventType.ICCUSendingOfObjectStateFailed:
                    return HandleICCUSendingOfObjectStateFailed();

                case EventType.ICCUPortAlreadyUsed:
                    return HandleICCUPortAlreadyUsed();

                case EventType.ExceptionOccurred:
                    return HandleExceptionOccured();

                case EventType.CCUMemoryLoadStateChanged:
                    return HandleCCUMemoryLoadStateChanged();

                case EventType.CCUFilesystemProblem:
                    return HandleCCUFilesystemProblem();

                case EventType.CCUSdCardNotFound:
                    return HandleCCUSdCardNotFound();

                case EventType.AlarmAreaBoughtTimeChanged:
                    return HandleAlarmAreaBoughtTimeChanged(this);

                case EventType.AlarmAreaBoughtTimeExpired:
                    return HandleAlarmAreaBoughtTimeExpired(this);

                case EventType.AlarmAreaTimeBuyingFailed:
                    return HandleAlarmAreaTimeBuyingFailed(this);

                case EventType.AntiPassBackZoneCardEntered:
                    return HandleAntiPassBackZoneCardEntered(this);

                case EventType.AntiPassBackZoneCardExited:
                    return HandleAntiPassBackZoneCardExited(this);

                case EventType.AntiPassBackZoneCardTimedOut:
                    return HandleAntiPassBackZoneCardTimedOut(this);

                case EventType.FunctionKeyPressed:
                    return HandleFunctionKeyPressed();

                case EventType.SetAlarmAreaFromClient:
                    return HandleSetAlarmAreaFromClient(this);

                case EventType.UnsetAlarmAreaFromClient:
                    return HandleUnsetAlarmAreaFromClient(this);
            }

            return true;
        }

        private bool SaveEventLogSetUnsetAAByObjectForAA(
            object objectGuid,
            DateTime dateTime,
            bool set,
            out Eventlog outEventlog,
            out List<EventSource> outEventSources,
            out List<EventlogParameter> outEventlogParameters,
            object[] parameters)
        {
            outEventlog = null;
            outEventSources = new List<EventSource>();
            outEventlogParameters = new List<EventlogParameter>();

            if (parameters == null || parameters.Length <= 0 ||
                    !(parameters[0] is Guid) ||
                    (Guid)parameters[0] == Guid.Empty)
                return false;

            var alarmArea = AlarmAreas.Singleton.GetById(objectGuid);
            if (alarmArea == null)
                return false;

            IEnumerable<Guid> eventSources = null;
            var objectName = string.Empty;

            if (parameters.Length > 1 && parameters[1] is ObjectType)
                switch ((ObjectType)parameters[1])
                {
                    case ObjectType.Input:
                        {
                            var input = Inputs.Singleton.GetById(parameters[0]);
                            if (input != null)
                                objectName = input.Name;
                            eventSources = CCUEventsManager.GetEventSourcesFromInput(input, (Guid)objectGuid);
                            break;
                        }

                    case ObjectType.Output:
                        {
                            var output = Outputs.Singleton.GetById(parameters[0]);
                            if (output != null)
                                objectName = output.Name;
                            eventSources = CCUEventsManager.GetEventSourcesFromOutput(output, (Guid)objectGuid);
                            break;
                        }

                    case ObjectType.TimeZone:
                        {
                            var timeZone = TimeZones.Singleton.GetById(parameters[0]);
                            if (timeZone != null)
                                objectName = timeZone.Name;
                            eventSources = new[] { (Guid)objectGuid, (Guid)parameters[0] };
                            break;
                        }

                    case ObjectType.DailyPlan:
                        {
                            var dailyPlan = DailyPlans.Singleton.GetById(parameters[0]);
                            if (dailyPlan != null)
                                objectName = dailyPlan.DailyPlanName;
                            eventSources = new[] { (Guid)objectGuid, (Guid)parameters[0] };
                            break;
                        }
                }

            var eventType =
                set
                    ? Eventlog.TYPESETALARMAREABYOBJECTFORAA
                    : Eventlog.TYPEUNSETALARMAREAFROMCARDREADER;

            var description =
                string.Format(
                    "{0} alarm area by object for automatic activation: {1}",
                    set
                        ? "Set"
                        : "Unset",
                    objectName);

            return Eventlogs.Singleton.CreateEvent(
                eventType,
                dateTime,
                _thisAssemblyName,
                eventSources,
                description,
                out outEventlog,
                out outEventSources,
                out outEventlogParameters);
        }

        private bool HandleUnsetAlarmAreaByObjectForAutomaticActivation()
        {
            if (_parameters == null ||
                    _parameters.Length <= 0 ||
                    !(_parameters[0] is Guid) ||
                    (Guid)_parameters[0] == Guid.Empty)
                return true;

            return SaveEventLogSetUnsetAAByObjectForAA(
                _objectGuid,
                _dateTime,
                false,
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters,
                _parameters);
        }

        private bool HandleSetAlarmAreaByObjectForAutomaticActivation()
        {
            return SaveEventLogSetUnsetAAByObjectForAA(
                _objectGuid,
                _dateTime,
                true,
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters,
                _parameters);
        }

        private bool HandleICCUPortAlreadyUsed()
        {
            var eventSources = new List<Guid>();

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
            if (ccu == null)
                return true;

            eventSources.Add(ccu.IdCCU);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_ICCU_PORT_ALREADY_USED,
                _dateTime,
                _thisAssemblyName,
                eventSources.ToArray(),
                "ICCU: Port for ICCU communication is already used",
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters);
        }

        private bool HandleICCUSendingOfObjectStateFailed()
        {
            if (_parameters == null ||
                    _parameters.Length <= 0 ||
                    !(_parameters[0] is int))
                return true;

            var eventSources = new List<Guid>();

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
            if (ccu == null)
                return true;

            eventSources.Add(ccu.IdCCU);

            switch ((ObjectType)_parameters[0])
            {
                case ObjectType.Input:

                    var input = Inputs.Singleton.GetById(_objectGuid);

                    if (input != null &&
                        input.DCU != null)
                        eventSources.Add(input.DCU.IdDCU);
                    break;

                case ObjectType.Output:

                    var output = Outputs.Singleton.GetById(_objectGuid);

                    if (output != null &&
                        output.DCU != null)
                        eventSources.Add(output.DCU.IdDCU);
                    break;
            }

            eventSources.Add(_objectGuid);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_ICCU_SENDING_OF_OBJECT_STATE_FAILED,
                _dateTime,
                _thisAssemblyName,
                eventSources.ToArray(),
                "ICCU: Sending of object state failed",
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters);
        }

        private bool HandleNotConfirmSetUnsetAAFromEIS()
        {
            var eventSources = new List<Guid>();

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
            if (ccu == null)
                return true;

            eventSources.Add(ccu.IdCCU);
            eventSources.Add(_objectGuid);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_ALARM_AREA_SETUNSET_NOT_RESPOND,
                _dateTime,
                _thisAssemblyName,
                eventSources.ToArray(),
                "External alarm system did not respond in proper time",
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters);
        }

        private bool HandleCCUIncomingTransferInfo()
        {
            if (_parameters == null ||
                    _parameters.Length != 2 ||
                    !(_parameters[0] is string) ||
                    !(_parameters[1] is string))
                return true;

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
            if (ccu == null)
                return true;

            var streamName = _parameters[0] as string;
            var duration = _parameters[1] as string;

            CCUConfigurationHandler.Singleton.TCPTransferSucceeded(ccu, ccu.IdCCU, streamName);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_INCOMING_TRANSFER_INFO,
                _dateTime,
                _thisAssemblyName,
                new[] {ccu.IdCCU},
                _parameters[0] as string,
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters,
                EventlogParameter.TYPE_STREAM_NAME,
                streamName,
                EventlogParameter.TYPE_DURATION,
                duration);
        }

        private bool HandleGetFromDatabaseReturnNull()
        {
            if (_parameters == null ||
                    _parameters.Length != 1 ||
                    !(_parameters[0] is int))
                return true;

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
            if (ccu == null)
                return true;

            var eventSources = new List<Guid>
                {
                    ccu.IdCCU
                };

            var objectType = (ObjectType)_parameters[0];
            if (objectType != ObjectType.CCU || _objectGuid != ccu.IdCCU)
                eventSources.Add(_objectGuid);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_GETFROMDATABASE_RETURN_NULL,
                _dateTime,
                _thisAssemblyName,
                eventSources,
                string.Format(
                    "The CCU returned null in loading the object of type {0} with id {1}",
                    objectType,
                    _objectGuid),
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters);
        }

        private bool HandleSendTamper()
        {
            if (_parameters == null ||
                _parameters.Length != 2 ||
                !(_parameters[0] is byte) ||
                !(_parameters[1] is bool))
            {
                return true;
            }

            var eventSources = Enumerable.Empty<Guid>();
            var eventlogType = string.Empty;
            var description = string.Empty;

            switch ((byte)_parameters[0])
            {
                case (byte)ObjectType.CCU:

                    var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
                    if (ccu == null)
                        break;

                    eventSources = new[] { ccu.IdCCU };
                    eventlogType = Eventlog.TYPE_CCU_TAMPER_STATE_CHANGED;

                    description =
                        string.Format(
                            "Tamper on the CCU is in state {0}",
                            (bool)_parameters[1]
                                ? "alarm"
                                : "normal");
                    break;

                case (byte)ObjectType.DCU:

                    var dcu = DCUs.Singleton.GetById(_objectGuid);
                    if (dcu == null)
                        break;

                    eventSources = CCUEventsManager.GetEventSourcesFromDCU(dcu);

                    eventlogType = Eventlog.TYPE_DCU_TAMPER_STATE_CHANGED;

                    description =
                        string.Format(
                            "Tamper on the DCU is in state {0}",
                            (bool)_parameters[1]
                                ? "alarm"
                                : "normal");

                    break;

                case (byte)ObjectType.CardReader:

                    var cardReader = CardReaders.Singleton.GetById(_objectGuid);
                    if (cardReader == null)
                        break;

                    eventSources = CCUEventsManager.GetEventSourcesFromCardReader(cardReader);

                    eventlogType = Eventlog.TYPE_CARD_READER_TAMPER_STATE_CHANGED;
                    description =
                        string.Format(
                            "Tamper on the card reader is in state {0}",
                            (bool)_parameters[1]
                                ? "alarm"
                                : "normal");

                    break;
            }

            if (eventlogType == string.Empty)
                return true;

            return Eventlogs.Singleton.CreateEvent(
                eventlogType,
                _dateTime,
                _thisAssemblyName,
                eventSources,
                description,
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters);
        }

        private bool HandleAlarmCCUExtFuse()
        {
            if (_parameters == null ||
                    _parameters.Length != 1 ||
                    !(_parameters[0] is bool))
                return true;

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
            if (ccu == null)
                return true;

            var description =
                string.Format(
                    "Alarm fuse on extension board on the CCU is in state {0}",
                    ((bool)_parameters[0]
                        ? "alarm"
                        : "normal"));

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_ALARM_FUSE_ON_EXTENSION_BOARD,
                _dateTime,
                _thisAssemblyName,
                new[] { ccu.IdCCU },
                description,
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters);
        }

        private bool HandleAlarmCCUBatteryIsLow()
        {
            if (_parameters == null ||
                    _parameters.Length != 1 ||
                    !(_parameters[0] is bool))
                return true;

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
            if (ccu == null)
                return true;

            var description =
                string.Format(
                    "Alarm battery is low on the CCU is in state {0}",
                    (bool)_parameters[0]
                        ? "alarm"
                        : "normal");

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_ALARM_BATTERY_IS_LOW,
                _dateTime,
                _thisAssemblyName,
                new[] { ccu.IdCCU },
                description,
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters);
        }

        private bool HandleAlarmCCUPrimaryPowerMissing()
        {
            if (_parameters == null ||
                    _parameters.Length != 1 ||
                    !(_parameters[0] is bool))
                return true;

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
            if (ccu == null)
                return true;

            var description =
                string.Format(
                    "Alarm primary power missing on the CCU is in state {0}",
                    (bool)_parameters[0]
                        ? "alarm"
                        : "normal");

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_ALARM_PRIMARY_POWER_MISSING,
                _dateTime,
                _thisAssemblyName,
                new[] { ccu.IdCCU },
                description,
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters);
        }

        private bool HandleDSMStateChanged()
        {
            if (_parameters == null ||
                    _parameters.Length != 1 ||
                    !(_parameters[0] is DoorEnvironmentState))
                return true;

            IEnumerable<Guid> eventSources;
            var doorEnvironment = DoorEnvironments.Singleton.GetById(_objectGuid);
            if (doorEnvironment != null)
            {
                eventSources = CCUEventsManager.GetEventSourcesFromDoorEnvironment(doorEnvironment);
            }
            else
            {
                var multiDoorElement = MultiDoorElements.Singleton.GetById(_objectGuid);
                if (multiDoorElement == null)
                    return true;

                eventSources = CCUEventsManager.GetEventSourcesFromMultiDoorElement(multiDoorElement);
            }

            var doorEnvironmentState = (DoorEnvironmentState)_parameters[0];

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_DSM_STATE_CHANGED,
                _dateTime,
                _thisAssemblyName,
                eventSources,
                string.Format(
                    "DSM state was changed. Actual DSM state is: {0}",
                    doorEnvironmentState),
                out _outEventlog,
                out _outEventSources);
        }

        private bool HandleCCUTimingProblem()
        {
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
            if (ccu == null)
                return true;

            if (_objectGuid == Guid.Empty)
                return true;

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_TIMING_PROBLEM,
                _dateTime,
                _thisAssemblyName,
                new[] { ccu.IdCCU, _objectGuid },
                "CCU timing problem",
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters);
        }

        private bool HandleObjectDeserializeFailed()
        {
            if (_parameters == null ||
                    _parameters.Length != 3 ||
                    !(_parameters[0] is int) ||
                    !(_parameters[1] is string) ||
                    !(_parameters[2] is string))
                return true;

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
            if (ccu == null)
                return true;

            ICollection<Guid> eventSources =
                new LinkedList<Guid>(
                    Enumerable.Repeat(ccu.IdCCU, 1));

            var objectType = (ObjectType)_parameters[0];

            var description = new StringBuilder("Failed to deserialize ");

            if (_objectGuid != Guid.Empty)
            {
                if (objectType != ObjectType.CCU ||
                    _objectGuid != ccu.IdCCU)
                    eventSources.Add(_objectGuid);

                description.AppendFormat(
                    "the object of type {0} with id {1} ",
                    objectType,
                    _objectGuid);
            }
            else
            {
                description.Append("an object ");

                if (objectType != ObjectType.NotSupport)
                    description.AppendFormat(
                        "of type {0} ",
                        objectType);
            }

            description.AppendFormat(
                "in method: {0}, with error: {1}",
                _parameters[1],
                _parameters[2]);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_OBJECT_DESERIALIZE_FAILED,
                _dateTime,
                _thisAssemblyName,
                eventSources,
                description.ToString(),
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters);
        }

        private bool HandleCCUTimeAdjusted()
        {
            if (_parameters == null ||
                    _parameters.Length != 2 ||
                    !(_parameters[0] is DateTime) ||
                    !(_parameters[1] is DateTime))
                return true;

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
            if (ccu == null)
                return true;

            var previousTime = ((DateTime)_parameters[0]).ToString("dd.MM.yyyy HH:mm:ss");
            var newTime = ((DateTime)_parameters[1]).ToString("dd.MM.yyyy HH:mm:ss");

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_TIME_ADJUSTED,
                _dateTime,
                _thisAssemblyName,
                new[] { ccu.IdCCU },
                string.Format(
                    "CCU adjusted its date/time from {0} to {1} ",
                    previousTime,
                    newTime),
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters);
        }

        private bool HandleCoprocessorFailureChanged()
        {
            if (_parameters == null ||
                    _parameters.Length != 1 ||
                    !(_parameters[0] is State))
                return true;

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
            if (ccu == null)
                return true;

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_COPROCESSOR_FAILURE_CHANGED,
                _dateTime,
                _thisAssemblyName,
                new[] { ccu.IdCCU },
                string.Format(
                    "CLSP coprocessor is {0}working properly: {1}",
                    (State)_parameters[0] == State.Alarm
                        ? "not "
                        : "",
                    ccu.Name),
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters);
        }

        private bool HandleSecurityTimeChannelChanged()
        {
            var eventSources = new List<Guid>();

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
            if (ccu != null)
                eventSources.Add(ccu.IdCCU);

            if (_objectGuid == Guid.Empty)
            {
                if (_parameters == null ||
                        _parameters.Length != 3 ||
                        _parameters[1] == null ||
                        _parameters[2] == null)
                    return true;

                return
                    Eventlogs.Singleton.CreateEvent(
                        Eventlog.TYPE_TIME_FOR_NEXT_EVALUATING_STATES_OF_SDP,
                        _dateTime,
                        _thisAssemblyName,
                        eventSources,
                        string.Format(
                            "Time for next evaluating states of security daily plans is {0}",
                            _parameters[1]),
                        out _outEventlog,
                        out _outEventSources,
                        out _outEventlogParameters);
            }

            if (_parameters == null ||
                    _parameters.Length != 2 ||
                    !(_parameters[0] is State) ||
                    !(_parameters[1] is byte))
                return true;

            switch ((byte)_parameters[1])
            {
                case (byte)ObjectType.SecurityDailyPlan:

                    var securityDailyPlan =
                        SecurityDailyPlans.Singleton.GetById(_objectGuid);

                    if (securityDailyPlan == null)
                        return true;

                    eventSources.Add(securityDailyPlan.IdSecurityDailyPlan);

                    return
                        Eventlogs.Singleton.CreateEvent(
                            Eventlog.TYPE_ACTUAL_STATE_OF_SECURITY_DAILY_PLAN,
                            _dateTime,
                            _thisAssemblyName,
                            eventSources,
                            string.Format(
                                "Actual state of securtiy daily plan {0} is {1}",
                                securityDailyPlan,
                                _parameters[0]),
                            out _outEventlog,
                            out _outEventSources,
                            out _outEventlogParameters);

                case (byte)ObjectType.SecurityTimeZone:

                    var securityTimeZone =
                        SecurityTimeZones.Singleton.GetById(_objectGuid);

                    if (securityTimeZone == null)
                        return true;

                    eventSources.Add(securityTimeZone.IdSecurityTimeZone);

                    return
                        Eventlogs.Singleton.CreateEvent(
                            Eventlog.TYPE_ACTUAL_STATE_OF_SECURITY_TIME_ZONE,
                            _dateTime,
                            _thisAssemblyName,
                            eventSources,
                            string.Format(
                                "Actual state of securtiy time zone {0} is {1}",
                                securityTimeZone,
                                _parameters[0]),
                            out _outEventlog,
                            out _outEventSources,
                            out _outEventlogParameters);
            }

            return true;
        }

        private bool HandleRunMethodFailed()
        {
            if (_parameters.Length <= 1 ||
                    !(_parameters[0] is string) ||
                    !(_parameters[1] is string))
                return true;

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);

            var eventSources =
                new LinkedList<Guid>(
                    ccu != null
                        ? Enumerable.Repeat(ccu.IdCCU, 1)
                        : Enumerable.Empty<Guid>());

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPERUNMETHODFAILED,
                _dateTime,
                _thisAssemblyName,
                eventSources,
                string.Format(
                    "Run method {0} on the ccu failed with exception: {1}",
                    _parameters[0],
                    _parameters[1]),
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters,
                EventlogParameter.TYPECCU,
                _ccuIpAddress);
        }

        private bool HandleExceptionOccured()
        {
            if (_parameters.Length < 1)
                return true;

            var exception = _parameters[0] as Exception;

            if (exception == null)
                return true;

            string stackTrace = null;

            if (_parameters.Length >= 2)
                stackTrace = (string)_parameters[1];

            var threadId = string.Empty;
            if (_parameters.Length >= 3 && _parameters[2] is int)
                threadId = ((int)_parameters[2]).ToString("x8");

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);

            var eventSources =
                new LinkedList<Guid>(
                    ccu != null
                        ? Enumerable.Repeat(ccu.IdCCU, 1)
                        : Enumerable.Empty<Guid>());

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_EXCEPTION_OCCURRED,
                _dateTime,
                _thisAssemblyName,
                eventSources,
                string.Format(
                    "CCU exception occurred : {0}",
                    exception),
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters,
                EventlogParameter.TYPE_STACKTRACE,
                stackTrace ?? "",
                EventlogParameter.TYPE_THREAD_ID,
                threadId);
        }

        private bool HandleSectorCardSystemRemoved()
        {
            var cardSystem = CardSystems.Singleton.GetById(_objectGuid);
            if (cardSystem == null)
                return true;

            var eventSources =
                new List<Guid>
                {
                    cardSystem.IdCardSystem
                };

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
            if (ccu != null)
                eventSources.Add(ccu.IdCCU);

            var description = new StringBuilder();

            description.AppendFormat(
                "Card system  {0} was removed",
                cardSystem);

            if (ccu != null)
                description.AppendFormat(" from CCU: {0}", ccu);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CARDSYSTEM_REMOVED,
                _dateTime,
                _thisAssemblyName,
                eventSources,
                description.ToString(),
                out _outEventlog,
                out _outEventSources);
        }

        private bool HandleSectorCardSystemAdded()
        {
            var cardSystem = CardSystems.Singleton.GetById(_objectGuid);
            if (cardSystem == null)
                return true;

            var eventSources =
                new List<Guid>
                {
                    cardSystem.IdCardSystem
                };

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
            if (ccu != null)
                eventSources.Add(ccu.IdCCU);

            var description = new StringBuilder();

            description.AppendFormat(
                "Card system  {0} was added",
                cardSystem);

            if (ccu != null)
                description.AppendFormat(
                    " to CCU: {0}",
                    ccu);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CARDSYSTEM_ADDED,
                _dateTime,
                _thisAssemblyName,
                eventSources,
                description.ToString(),
                out _outEventlog,
                out _outEventSources);
        }

        private bool HandleDcuNodeReleased()
        {
            var dcu = DCUs.Singleton.GetById(_objectGuid);
            if (dcu == null)
                return true;

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDCUNODRELEASED,
                _dateTime,
                _thisAssemblyName,
                CCUEventsManager.GetEventSourcesFromDCU(dcu),
                "DCU released ",
                out _outEventlog,
                out _outEventSources);
        }

        private bool HandleDcuNodeRenewed()
        {
            var dcu = DCUs.Singleton.GetById(_objectGuid);
            if (dcu == null)
                return true;

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDCUNODRENEWED,
                _dateTime,
                _thisAssemblyName,
                CCUEventsManager.GetEventSourcesFromDCU(dcu),
                "DCU renewed ",
                out _outEventlog,
                out _outEventSources);
        }

        private bool HandleDcuNodeAssigned()
        {
            var dcu = DCUs.Singleton.GetById(_objectGuid);
            if (dcu == null)
                return true;

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDCUNODASSIGNED,
                _dateTime,
                _thisAssemblyName,
                CCUEventsManager.GetEventSourcesFromDCU(dcu),
                "DCU assigned ",
                out _outEventlog,
                out _outEventSources);
        }

        private bool HandleDcuCommandNotACK()
        {
            if (_parameters == null ||
                _parameters.Length == 0 ||
                !(_parameters[0] is string) ||
                (_parameters.Length > 1 && !(_parameters[1] is string)))
                return true;

            var dcu = DCUs.Singleton.GetById(_objectGuid);
            if (dcu == null)
                return true;

            var parameters = _parameters.Length == 1
                ? new[]
                {
                    EventlogParameter.TYPE_REASON,
                    (string) _parameters[0]
                }
                : new[]
                {
                    EventlogParameter.TYPE_REASON,
                    (string) _parameters[0],
                    EventlogParameter.TYPE_COMMAND,
                    (string) _parameters[1]
                };

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDCUCOMMANDNACK,
                _dateTime,
                _thisAssemblyName,
                CCUEventsManager.GetEventSourcesFromDCU(dcu),
                "DCU command not ack ",
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters,
                parameters);
        }

        private bool HandleDcuCommandTimeOut()
        {
            var commandName = string.Empty;
            if (_parameters.Length > 0)
                commandName = (string)_parameters[0];

            var dcu = DCUs.Singleton.GetById(_objectGuid);
            if (dcu == null)
                return true;

            var description = new StringBuilder("DCU command timeout: ");

            description.Append(
                commandName == string.Empty
                    ? "unknown"
                    : commandName);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDCUCOMMANDTIMEOUT,
                _dateTime,
                _thisAssemblyName,
                CCUEventsManager.GetEventSourcesFromDCU(dcu),
                description.ToString(),
                out _outEventlog,
                out _outEventSources);
        }

        private bool HandleDSMApasRestored()
        {
            var doorEnvironment = DoorEnvironments.Singleton.GetById(_objectGuid);
            if (doorEnvironment == null)
                return true;

            IEnumerable<Guid> eventSources;

            var newParameters =
                CreateDsmEventsParameters(
                    _parameters,
                    CCUEventsManager.GetEventSourcesFromDoorEnvironment(doorEnvironment),
                    out eventSources);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDSMAPASRESTORED,
                _dateTime,
                _thisAssemblyName,
                eventSources,
                "APAS restored ",
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters,
                newParameters);
        }

        private bool HandleDSMAccessViolated()
        {
            var doorEnvironment = DoorEnvironments.Singleton.GetById(_objectGuid);
            if (doorEnvironment == null)
                return true;

            IEnumerable<Guid> eventSources;

            var newParameters =
                CreateDsmEventsParameters(
                    _parameters,
                    CCUEventsManager.GetEventSourcesFromDoorEnvironment(doorEnvironment),
                    out eventSources);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDSMACCESSVIOLATED,
                _dateTime,
                _thisAssemblyName,
                eventSources,
                "Access violated ",
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters,
                newParameters);
        }

        private bool HandleDSMAccessRestricted()
        {
            var doorEnvironment = DoorEnvironments.Singleton.GetById(_objectGuid);
            if (doorEnvironment == null)
                return true;

            IEnumerable<Guid> eventSources;

            var newParameters =
                CreateDsmEventsParameters(
                    _parameters,
                    CCUEventsManager.GetEventSourcesFromDoorEnvironment(doorEnvironment),
                    out eventSources);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDSMACCESSRESTRICTED,
                _dateTime,
                _thisAssemblyName,
                eventSources,
                "Access restricted",
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters,
                newParameters);
        }

        private bool HandleDSMAccessInterupted()
        {
            var doorEnvironment = DoorEnvironments.Singleton.GetById(_objectGuid);
            if (doorEnvironment == null)
                return true;

            IEnumerable<Guid> eventSources;

            var newParameters =
                CreateDsmEventsParameters(
                    _parameters,
                    CCUEventsManager.GetEventSourcesFromDoorEnvironment(doorEnvironment),
                    out eventSources);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDSMACCESSINTERRUPTED,
                _dateTime,
                _thisAssemblyName,
                eventSources,
                "Access interrupted",
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters,
                newParameters);
        }

        private bool HandleDSMNormalAccess()
        {
            var doorEnvironment = DoorEnvironments.Singleton.GetById(_objectGuid);
            if (doorEnvironment == null)
                return true;

            IEnumerable<Guid> eventSources;

            var newParameters =
                CreateDsmEventsParameters(
                    _parameters,
                    CCUEventsManager.GetEventSourcesFromDoorEnvironment(doorEnvironment),
                    out eventSources);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEDSMNORMALACCESS,
                    _dateTime,
                    _thisAssemblyName,
                    eventSources,
                    "Normal access",
                    out _outEventlog,
                    out _outEventSources,
                    out _outEventlogParameters,
                    newParameters);
        }

        private bool HandleDSMAccessPermitted()
        {
            if (_parameters == null ||
                    _parameters.Length <= 1 ||
                    !(_parameters[_parameters.Length - 1] is bool))
                return true;

            IEnumerable<Guid> eventSources;
            var doorEnvironment = DoorEnvironments.Singleton.GetById(_objectGuid);
            if (doorEnvironment != null)
            {
                eventSources = CCUEventsManager.GetEventSourcesFromDoorEnvironment(doorEnvironment);
            }
            else
            {
                var multiDoorElement = MultiDoorElements.Singleton.GetById(_objectGuid);
                if (multiDoorElement == null)
                    return true;

                eventSources = CCUEventsManager.GetEventSourcesFromMultiDoorElement(multiDoorElement);
            }

            var parametersWithoutLast = new object[_parameters.Length - 1];

            Array.Copy(
                _parameters, 0,
                parametersWithoutLast, 0,
                parametersWithoutLast.Length);

            IEnumerable<Guid> allEventSources;

            var newParameters =
                CreateDsmEventsParameters(
                    parametersWithoutLast,
                    eventSources,
                    out allEventSources);

            if ((bool)_parameters[_parameters.Length - 1])
            {
                CardReader cardReader = null;
                if (_parameters[0] is Guid)
                    cardReader = CardReaders.Singleton.GetById((Guid)_parameters[0]);

                Card card = null;
                if (_parameters[1] is Guid)
                    card = Cards.Singleton.GetById((Guid)_parameters[1]);

                if (cardReader != null)
                    CreateAlarmsForCardReaders.CreateAlarm(
                        _dateTime,
                        cardReader,
                        AlarmType.CardReader_AccessPermitted,
                        _ccuIpAddress,
                        card);
            }

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEDSMACCESSPERMITTED,
                    _dateTime,
                    _thisAssemblyName,
                    allEventSources,
                    "Access permitted",
                    out _outEventlog,
                    out _outEventSources,
                    out _outEventlogParameters,
                    newParameters);
        }

        private bool HandleUnsetAlarmAreaFromCardReader()
        {
            if (_parameters == null ||
                    _parameters.Length <= 0 ||
                    !(_parameters[0] is Guid) ||
                    (Guid)_parameters[0] == Guid.Empty)
                return true;

            var cardReader = CardReaders.Singleton.GetById(_objectGuid);
            if (cardReader == null)
                return true;

            CCU ccu = null;
            if (cardReader.CCU != null)
                ccu = cardReader.CCU;
            else
                if (cardReader.DCU != null)
                    ccu = cardReader.DCU.CCU;

            if (ccu == null)
                return true;

            if (_parameters.Length > 1 &&
                _parameters[1] is Guid &&
                (Guid)_parameters[1] != Guid.Empty)
            {
                var card = Cards.Singleton.GetById((Guid)_parameters[0]);
                var alarmArea = AlarmAreas.Singleton.GetById((Guid)_parameters[1]);

                if (card == null || alarmArea == null)
                    return true;

                string personName;

                var eventSources =
                    GetEventSourcesFromCardReader(
                        cardReader,
                        card,
                        alarmArea,
                        out personName);

                return Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEUNSETALARMAREAFROMCARDREADER,
                    _dateTime,
                    _thisAssemblyName,
                    eventSources,
                    string.Format(
                        "Unset alarm area from card reader, card: {0}, person: {1}, alarm area: {2}",
                        card.GetFullCardNumber(),
                        personName,
                        alarmArea),
                    out _outEventlog,
                    out _outEventSources,
                    out _outEventlogParameters,
                    EventlogParameter.TYPECARDNUMBER,
                    card.GetFullCardNumber(),
                    EventlogParameter.TYPEPERSONNAME,
                    personName,
                    EventlogParameter.TYPEPEALARMAREANAME,
                    alarmArea.ToString());
            }
            else
            {
                var alarmArea = AlarmAreas.Singleton.GetById((Guid)_parameters[0]);
                if (alarmArea == null)
                    return true;

                return Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEUNSETALARMAREAFROMCARDREADER,
                    _dateTime,
                    _thisAssemblyName,
                    GetEventSourcesFromCardReader(cardReader, alarmArea),
                    string.Format(
                        "Unset alarm area from card reader with gin, alarm area: {0}",
                        alarmArea),
                    out _outEventlog,
                    out _outEventSources,
                    out _outEventlogParameters,
                    EventlogParameter.TYPEPEALARMAREANAME,
                    alarmArea.ToString());
            }
        }

        private bool HandleSetAlarmAreaFromCardReader()
        {
            if (_parameters == null ||
                    _parameters.Length <= 0 ||
                    !(_parameters[0] is Guid) ||
                    (Guid)_parameters[0] == Guid.Empty)
                return true;

            var cardReader = CardReaders.Singleton.GetById(_objectGuid);
            if (cardReader == null)
                return true;

            CCU ccu = null;
            if (cardReader.CCU != null)
                ccu = cardReader.CCU;
            else
                if (cardReader.DCU != null)
                    ccu = cardReader.DCU.CCU;

            if (ccu == null)
                return true;

            if (_parameters.Length > 1 &&
                _parameters[1] is Guid &&
                (Guid)_parameters[1] != Guid.Empty)
            {
                var card = Cards.Singleton.GetById((Guid)_parameters[0]);
                var alarmArea = AlarmAreas.Singleton.GetById((Guid)_parameters[1]);
                if (card == null || alarmArea == null)
                    return true;

                string personName;

                var eventSources =
                    GetEventSourcesFromCardReader(
                        cardReader,
                        card,
                        alarmArea,
                        out personName);

                return
                    Eventlogs.Singleton.CreateEvent(
                        Eventlog.TYPESETALARMAREAFROMCARDREADER,
                        _dateTime,
                        _thisAssemblyName,
                        eventSources,
                        string.Format(
                            "Set alarm area from card reader, card: {0}, person: {1}, alarm area: {2}",
                            card.GetFullCardNumber(),
                            personName,
                            alarmArea),
                        out _outEventlog,
                        out _outEventSources,
                        out _outEventlogParameters,
                        EventlogParameter.TYPECARDNUMBER,
                        card.GetFullCardNumber(),
                        EventlogParameter.TYPEPERSONNAME,
                        personName,
                        EventlogParameter.TYPEPEALARMAREANAME,
                        alarmArea.ToString());
            }
            else
            {
                var alarmArea = AlarmAreas.Singleton.GetById((Guid)_parameters[0]);
                if (alarmArea == null)
                    return true;

                return Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPESETALARMAREAFROMCARDREADER,
                    _dateTime,
                    _thisAssemblyName,
                    GetEventSourcesFromCardReader(cardReader, alarmArea),
                    string.Format(
                        "Set alarm area from card reader with gin, alarm area: {0}",
                        alarmArea),
                    out _outEventlog,
                    out _outEventSources,
                    out _outEventlogParameters,
                    EventlogParameter.TYPEPEALARMAREANAME,
                    alarmArea.ToString());
            }
        }

        private bool HandleAccessDeniedUnsetAlarmAreaInvalidGin()
        {
            if (_parameters == null ||
                    _parameters.Length <= 0 ||
                    !(_parameters[0] is Guid) ||
                    (Guid)_parameters[0] == Guid.Empty)
                return true;

            var alarmArea = AlarmAreas.Singleton.GetById((Guid)_parameters[0]);
            if (alarmArea == null)
                return true;

            var cardReader = CardReaders.Singleton.GetById(_objectGuid);
            if (cardReader == null)
                return true;

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEACCESSDENIEDUNSETALARMAREAINVALIDGIN,
                _dateTime,
                _thisAssemblyName,
                GetEventSourcesFromCardReader(cardReader, alarmArea),
                string.Format(
                    "Access dennied unset alarm area invalid gin, alarm area: {0}",
                    alarmArea),
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters,
                EventlogParameter.TYPEPEALARMAREANAME,
                alarmArea.ToString());
        }

        private bool HandleAccessDeniedSetAlarmAreaInvalidGin()
        {
            if (_parameters == null ||
                _parameters.Length <= 0 ||
                !(_parameters[0] is Guid) ||
                (Guid)_parameters[0] == Guid.Empty)
            {
                return true;
            }

            var alarmArea = AlarmAreas.Singleton.GetById((Guid)_parameters[0]);
            if (alarmArea == null)
                return true;

            var cardReader = CardReaders.Singleton.GetById(_objectGuid);
            if (cardReader == null)
                return true;

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEACCESSDENIEDSETALARMAREAINVALIDGIN,
                    _dateTime,
                    _thisAssemblyName,
                    GetEventSourcesFromCardReader(cardReader, alarmArea),
                    string.Format(
                        "Access dennied set alarm area invalid gin, alarm area: {0}",
                        alarmArea),
                    out _outEventlog,
                    out _outEventSources,
                    out _outEventlogParameters,
                    EventlogParameter.TYPEPEALARMAREANAME,
                    alarmArea.ToString());
        }

        private bool HandleAlarmAreaSetFromCRFailed()
        {
            if (_parameters == null ||
                _parameters.Length <= 1 ||
                !(_parameters[0] is Guid) || (Guid)_parameters[0] == Guid.Empty ||
                !(_parameters[1] is Guid) || (Guid)_parameters[1] == Guid.Empty)
            {
                return true;
            }

            var card = Cards.Singleton.GetById((Guid)_parameters[0]);
            var alarmArea = AlarmAreas.Singleton.GetById((Guid)_parameters[1]);
            if (card == null || alarmArea == null)
                return true;

            var cardReader = CardReaders.Singleton.GetById(_objectGuid);
            if (cardReader == null)
                return true;

            string personName;

            var eventSources =
                GetEventSourcesFromCardReader(
                    cardReader,
                    card,
                    alarmArea,
                    out personName);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_ALARM_AREA_SET_FROM_CR_FAILED,
                _dateTime,
                _thisAssemblyName,
                eventSources,
                string.Format(
                    "Set alarm area from card reader failed, card reader: {0}, card: {1}, person: {2}, alarm area: {3}",
                    cardReader.Name,
                    card.GetFullCardNumber(),
                    personName,
                    alarmArea),
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters,
                EventlogParameter.TYPECARDNUMBER,
                card.GetFullCardNumber(),
                EventlogParameter.TYPEPERSONNAME,
                personName,
                EventlogParameter.TYPEPEALARMAREANAME,
                alarmArea.ToString());
        }

        private bool HandleAccessDeniedUnsetAlarmAreaNoRights()
        {
            if (_parameters == null ||
                _parameters.Length <= 1 ||
                !(_parameters[0] is Guid) ||
                (Guid)_parameters[0] == Guid.Empty ||
                !(_parameters[1] is Guid) ||
                (Guid)_parameters[1] == Guid.Empty)
            {
                return true;
            }
            var card = Cards.Singleton.GetById((Guid)_parameters[0]);
            var alarmArea = AlarmAreas.Singleton.GetById((Guid)_parameters[1]);

            if (card == null || alarmArea == null)
                return true;

            var cardReader = CardReaders.Singleton.GetById(_objectGuid);
            if (cardReader == null)
                return true;

            string personName;

            var eventSources =
                GetEventSourcesFromCardReader(
                    cardReader,
                    card,
                    alarmArea,
                    out personName);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEACCESSDENIEDUNSETALARMAREANORIGHTS,
                    _dateTime,
                    _thisAssemblyName,
                    eventSources,
                    string.Format(
                        "Access dennied unset alarm area no rights, card: {0}, person: {1}, alarm area: {2}",
                        card.GetFullCardNumber(),
                        personName,
                        alarmArea),
                    out _outEventlog,
                    out _outEventSources,
                    out _outEventlogParameters,
                    EventlogParameter.TYPECARDNUMBER,
                    card.GetFullCardNumber(),
                    EventlogParameter.TYPEPERSONNAME,
                    personName,
                    EventlogParameter.TYPEPEALARMAREANAME,
                    alarmArea.ToString());
        }

        private bool HandleAccessDeniedSetAlarmAreaNoRights()
        {
            if (_parameters == null ||
                    _parameters.Length <= 1 ||
                    !(_parameters[0] is Guid) ||
                    (Guid)_parameters[0] == Guid.Empty ||
                    !(_parameters[1] is Guid) ||
                    (Guid)_parameters[1] == Guid.Empty)
                return true;

            var card = Cards.Singleton.GetById((Guid)_parameters[0]);
            var alarmArea = AlarmAreas.Singleton.GetById((Guid)_parameters[1]);

            if (card == null || alarmArea == null)
                return true;

            var cardReader = CardReaders.Singleton.GetById(_objectGuid);
            if (cardReader == null)
                return true;

            string personName;

            var eventSources =
                GetEventSourcesFromCardReader(
                    cardReader,
                    card,
                    alarmArea,
                    out personName);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEACCESSDENIEDSETALARMAREANORIGHTS,
                    _dateTime,
                    _thisAssemblyName,
                    eventSources,
                    string.Format(
                        "Access dennied set alarm area no rights, card: {0}, person: {1}, alarm area: {2}",
                        card.GetFullCardNumber(),
                        personName,
                        alarmArea),
                    out _outEventlog,
                    out _outEventSources,
                    out _outEventlogParameters,
                    EventlogParameter.TYPECARDNUMBER,
                    card.GetFullCardNumber(),
                    EventlogParameter.TYPEPERSONNAME,
                    personName,
                    EventlogParameter.TYPEPEALARMAREANAME,
                    alarmArea.ToString());
        }

        private bool HandleAccessDeniedUnsetAlarmAreaInvalidPin()
        {
            if (_parameters == null || _parameters.Length <= 1 ||
                    !(_parameters[0] is Guid) || (Guid)_parameters[0] == Guid.Empty ||
                    !(_parameters[1] is Guid) || (Guid)_parameters[1] == Guid.Empty)
                return true;

            var card = Cards.Singleton.GetById((Guid)_parameters[0]);
            var alarmArea = AlarmAreas.Singleton.GetById((Guid)_parameters[1]);

            if (card == null || alarmArea == null)
                return true;

            var cardReader = CardReaders.Singleton.GetById(_objectGuid);
            if (cardReader == null)
                return true;

            string personName;

            var eventSources =
                GetEventSourcesFromCardReader(
                    cardReader,
                    card,
                    alarmArea,
                    out personName);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEACCESSDENIEDUNSETALARMAREAINVALIDPIN,
                    _dateTime,
                    _thisAssemblyName,
                    eventSources,
                    string.Format(
                        "Access dennied unset alarm area invalid pin, card: {0}, person: {1}, alarm area: {2}",
                        card.GetFullCardNumber(),
                        personName,
                        alarmArea),
                    out _outEventlog,
                    out _outEventSources,
                    out _outEventlogParameters,
                    EventlogParameter.TYPECARDNUMBER,
                    card.GetFullCardNumber(),
                    EventlogParameter.TYPEPERSONNAME,
                    personName,
                    EventlogParameter.TYPEPEALARMAREANAME,
                    alarmArea.ToString());
        }

        private bool HandleAccessDeniedSetAlarmAreaInvalidPin()
        {
            if (_parameters == null || _parameters.Length <= 1 ||
                    !(_parameters[0] is Guid) || (Guid)_parameters[0] == Guid.Empty ||
                    !(_parameters[1] is Guid) || (Guid)_parameters[1] == Guid.Empty)
                return true;

            var card = Cards.Singleton.GetById((Guid)_parameters[0]);
            var alarmArea = AlarmAreas.Singleton.GetById((Guid)_parameters[1]);

            if (card == null || alarmArea == null)
                return true;

            var cardReader = CardReaders.Singleton.GetById(_objectGuid);
            if (cardReader == null)
                return true;

            string personName;

            var eventSources =
                GetEventSourcesFromCardReader(
                    cardReader,
                    card,
                    alarmArea,
                    out personName);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEACCESSDENIEDSETALARMAREAINVALIDPIN,
                    _dateTime,
                    _thisAssemblyName,
                    eventSources,
                    string.Format(
                        "Access dennied set alarm area invalid pin, card: {0}, person: {1}, alarm area: {2}",
                        card.GetFullCardNumber(),
                        personName,
                        alarmArea),
                    out _outEventlog,
                    out _outEventSources,
                    out _outEventlogParameters,
                    EventlogParameter.TYPECARDNUMBER,
                    card.GetFullCardNumber(),
                    EventlogParameter.TYPEPERSONNAME,
                    personName,
                    EventlogParameter.TYPEPEALARMAREANAME,
                    alarmArea.ToString());
        }

        private static ICollection<Guid> GetEventSourcesFromCardReaderInternal(
            CardReader cardReader,
            Card card,
            out string personName)
        {
            ICollection<Guid> eventSources =
                new LinkedList<Guid>(
                    CCUEventsManager.GetEventSourcesFromCardReader(cardReader));

            personName = string.Empty;
            if (card != null)
            {
                eventSources.Add(card.IdCard);
                
                if (card.Person != null)
                {
                    eventSources.Add(card.Person.IdPerson);
                    personName = card.Person.ToString();
                }
            }

            return eventSources;
        }

        private static IEnumerable<Guid> GetEventSourcesFromCardReader(
            CardReader cardReader,
            Card card,
            out string personName)
        {
            return
                GetEventSourcesFromCardReaderInternal(
                    cardReader,
                    card,
                    out personName);
        }

        private static IEnumerable<Guid> GetEventSourcesFromCardReader(
            CardReader cardReader,
            Card card,
            AlarmArea alarmArea,
            out string personName)
        {
            var result =
                GetEventSourcesFromCardReaderInternal(
                    cardReader,
                    card,
                    out personName);

            result.Add(alarmArea.IdAlarmArea);

            return result;
        }

        private static IEnumerable<Guid> GetEventSourcesFromCardReader(
            CardReader cardReader,
            AlarmArea alarmArea)
        {
            ICollection<Guid> eventSources =
                new LinkedList<Guid>(
                    CCUEventsManager.GetEventSourcesFromCardReader(cardReader));

            eventSources.Add(alarmArea.IdAlarmArea);

            return eventSources;
        }

        private bool HandleAccessDeniedCardBlockedOrInactive()
        {
            if (_parameters == null || _parameters.Length != 2 ||
                    !(_parameters[0] is Guid) || (Guid)_parameters[0] == Guid.Empty ||
                    !(_parameters[1] is bool))
                return true;

            var card = Cards.Singleton.GetById((Guid)_parameters[0]);
            if (card == null)
                return true;

            var cardReader = CardReaders.Singleton.GetById(_objectGuid);
            if (cardReader == null)
                return true;

            string personName;

            var eventSources =
                GetEventSourcesFromCardReader(
                    cardReader,
                    card,
                    out personName);

            var retValue =
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEACCESSDENIEDCARDBLOCKEDORINACTIVE,
                    _dateTime,
                    _thisAssemblyName,
                    eventSources,
                    string.Format(
                        "Access dennied card blocked or inactive, card: {0}, person: {1}",
                        card.GetFullCardNumber(),
                        personName),
                    out _outEventlog,
                    out _outEventSources,
                    out _outEventlogParameters,
                    EventlogParameter.TYPECARDNUMBER,
                    card.GetFullCardNumber(),
                    EventlogParameter.TYPEPERSONNAME,
                    personName);

            if ((bool)_parameters[1])
                CreateAlarmsForCardReaders.CreateAlarm(
                    _dateTime,
                    cardReader,
                    AlarmType.CardReader_CardBlockedOrInactive,
                    _ccuIpAddress,
                    card);

            return retValue;
        }

        private bool HandleAccessDeniedInvalidEmergencyCode()
        {
            if (_parameters == null || _parameters.Length != 1 || !(_parameters[0] is bool))
                return true;

            var cardReader = CardReaders.Singleton.GetById(_objectGuid);
            if (cardReader == null)
                return true;

            var retValue =
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEACCESSDENIEDINVALIDEMERGENCYCODE,
                    _dateTime,
                    _thisAssemblyName,
                    CCUEventsManager.GetEventSourcesFromCardReader(cardReader),
                    "Access dennied invalid emergency code",
                    out _outEventlog,
                    out _outEventSources);

            if ((bool)_parameters[0])
                CreateAlarmsForCardReaders.CreateAlarm(
                    _dateTime,
                    cardReader,
                    AlarmType.CardReader_InvalidEmergencyCode,
                    _ccuIpAddress,
                    null);

            return retValue;
        }

        private bool HandleAccessDeniedInvalidGin()
        {
            if (_parameters == null || _parameters.Length != 1 || !(_parameters[0] is bool))
                return true;

            var cardReader = CardReaders.Singleton.GetById(_objectGuid);
            if (cardReader == null)
                return true;

            var retValue =
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEACCESSDENIEDINVALIDGIN,
                    _dateTime,
                    _thisAssemblyName,
                    CCUEventsManager.GetEventSourcesFromCardReader(cardReader),
                    "Access dennied invalid GIN",
                    out _outEventlog,
                    out _outEventSources);

            if ((bool)_parameters[0])
                CreateAlarmsForCardReaders.CreateAlarm(
                    _dateTime,
                    cardReader,
                    AlarmType.CardReader_InvalidGIN,
                    _ccuIpAddress,
                    null);

            return retValue;
        }

        private bool HandleAccessDeniedInvalidPin()
        {
            if (_parameters == null || _parameters.Length <= 0)
                return true;

            if (!(_parameters[0] is Guid) || (Guid)_parameters[0] == Guid.Empty)
                return true;

            var card = Cards.Singleton.GetById((Guid)_parameters[0]);
            if (card == null)
                return true;

            var cardReader = CardReaders.Singleton.GetById(_objectGuid);
            if (cardReader == null)
                return true;

            string personName;

            var eventSources =
                GetEventSourcesFromCardReader(
                    cardReader,
                    card,
                    out personName);

            var retValue =
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEACCESSDENIEDINVALIDPIN,
                    _dateTime,
                    _thisAssemblyName,
                    eventSources,
                    string.Format(
                        "Access dennied invalid PIN, card: {0}, person: {1}",
                        card.GetFullCardNumber(),
                        personName),
                    out _outEventlog,
                    out _outEventSources,
                    out _outEventlogParameters,
                    EventlogParameter.TYPECARDNUMBER,
                    card.GetFullCardNumber(),
                    EventlogParameter.TYPEPERSONNAME,
                    personName);

            if (_parameters.Length > 1 &&
                _parameters[1] is bool && (bool)_parameters[1])
                CreateAlarmsForCardReaders.CreateAlarm(
                    _dateTime,
                    cardReader,
                    AlarmType.CardReader_InvalidPIN,
                    _ccuIpAddress,
                    card);

            return retValue;
        }

        private bool HandleAccessDenied()
        {
            if (_parameters == null || _parameters.Length <= 0)
                return true;

            if (!(_parameters[0] is Guid) || (Guid)_parameters[0] == Guid.Empty)
                return true;

            var card = Cards.Singleton.GetById((Guid)_parameters[0]);
            if (card != null)
            {
                var cardReader = CardReaders.Singleton.GetById(_objectGuid);
                if (cardReader == null)
                    return true;

                string personName;

                var eventSources =
                    GetEventSourcesFromCardReader(
                        cardReader,
                        card,
                        out personName);

                var retValue =
                    Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEACCESSDENIED,
                    _dateTime,
                    _thisAssemblyName,
                    eventSources,
                    string.Format(
                        "Access dennied card: {0}, person: {1}",
                        card.GetFullCardNumber(),
                        personName),
                    out _outEventlog,
                    out _outEventSources,
                    out _outEventlogParameters,
                    EventlogParameter.TYPECARDNUMBER,
                    card.GetFullCardNumber(),
                    EventlogParameter.TYPEPERSONNAME,
                    personName);

                if (_parameters.Length > 1 &&
                        _parameters[1] is bool && (bool)_parameters[1])
                    CreateAlarmsForCardReaders.CreateAlarm(
                        _dateTime,
                        cardReader,
                        AlarmType.CardReader_AccessDenied,
                        _ccuIpAddress,
                        card);

                return retValue;
            }

            var input = Inputs.Singleton.GetById(_objectGuid);
            if (input == null)
                return true;

            var ccu =
                input.CCU ?? (input.DCU != null ? input.DCU.CCU : null);

            if (ccu == null)
                return true;

            var guidDoorEnvironment =
                _parameters.Length > 0 &&
                (Guid)_parameters[0] != Guid.Empty
                    ? (Guid)_parameters[0]
                    : Guid.Empty;

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEACCESSDENIED,
                    _dateTime,
                    _thisAssemblyName,
                    CCUEventsManager.GetEventSourcesFromInput(input, guidDoorEnvironment),
                    string.Format(
                        "Access denied with push button: {0} {1}",
                        input.Name,
                        _parameters.Length > 1 && _parameters[1] is bool && (bool)_parameters[1]
                            ? "alarm area is set"
                            : "door environment is locked"),
                    out _outEventlog,
                    out _outEventSources);
        }

        private bool HandleUnknownCard()
        {
            if (_parameters == null || _parameters.Length <= 0 || !(_parameters[0] is string))
                return true;

            var cardReader = CardReaders.Singleton.GetById(_objectGuid);
            if (cardReader == null)
                return false;

            ICollection<Guid> eventSources =
                new LinkedList<Guid>(
                    CCUEventsManager.GetEventSourcesFromCardReader(cardReader));

            var card = Cards.Singleton.GetCardFromFullCardNumber((string)_parameters[0]);

            object alarmObject;
            if (card != null)
            {
                eventSources.Add(card.IdCard);
                alarmObject = card;
            }
            else
                alarmObject = _parameters[0];

            var retValue = Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEUNKNOWNCARD,
                _dateTime,
                _thisAssemblyName,
                eventSources,
                string.Format("Unknown card: {0}", _parameters[0]),
                out _outEventlog,
                out _outEventSources,
                out _outEventlogParameters,
                EventlogParameter.TYPECARDNUMBER,
                (string)_parameters[0]);

            if (_parameters.Length > 1 &&
                    _parameters[1] is bool &&
                    (bool)_parameters[1])
                CreateAlarmsForCardReaders.CreateAlarm(
                    _dateTime,
                    cardReader,
                    AlarmType.CardReader_UnknownCard,
                    _ccuIpAddress,
                    alarmObject);


            return retValue;
        }

        private bool HandleAlarmAreaActivationStateChanged()
        {
            var alarmArea = AlarmAreas.Singleton.GetById(_objectGuid);

            if (alarmArea == null)
                return true;

            var state = ActivationState.Unknown;
            if (_parameters[0] is ActivationState)
                state = (ActivationState)_parameters[0];

            ICollection<Guid> eventSources = new LinkedList<Guid>();

            for (var i = 1; i < _parameters.Length; i++)
            {
                if (!(_parameters[i] is Guid))
                    continue;

                var cardReader = CardReaders.Singleton.GetById(_parameters[i]);
                if (cardReader != null)
                {
                    var cardReaderEventSources = GetEventSourcesFromCardReader(
                        cardReader,
                        alarmArea);

                    foreach (var cardReaderEventSource in cardReaderEventSources)
                    {
                        eventSources.Add(cardReaderEventSource);
                    }

                    continue;
                }

                var card = Cards.Singleton.GetById(_parameters[i]);

                if (card != null)
                {
                    eventSources.Add(card.IdCard);

                    if (card.Person != null)
                        eventSources.Add(card.Person.IdPerson);
                }
            }

            if (!eventSources.Contains(alarmArea.IdAlarmArea))
                eventSources.Add(alarmArea.IdAlarmArea);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEALARMAREAACTIVATIONSTATECHANGED,
                _dateTime,
                _thisAssemblyName,
                eventSources,
                string.Format(
                        "Alarm area {0} changed its activation state to {1}",
                        alarmArea.Name,
                        state),
                out _outEventlog,
                out _outEventSources);
        }

        private bool HandleAlarmAreaStateChanged()
        {
            if (_parameters == null || _parameters.Length != 1)
                return true;

            var alarmArea = AlarmAreas.Singleton.GetById(_objectGuid);
            if (alarmArea == null)
                return true;

            var state = AlarmAreaAlarmState.Unknown;
            if (_parameters[0] is AlarmAreaAlarmState)
                state = (AlarmAreaAlarmState)_parameters[0];

            var description =
                string.Format(
                    "Alarm area {0} changed its alarm state to {1}",
                    alarmArea.Name,
                    state);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_ALARM_AREA_ALARM_STATE_CHANGED,
                    _dateTime,
                    _thisAssemblyName,
                    new[] { alarmArea.IdAlarmArea },
                    description,
                    out _outEventlog,
                    out _outEventSources);
        }

        private bool HandleOutputStateChanged()
        {
            if (_parameters == null || _parameters.Length != 1)
                return true;

            var output = Outputs.Singleton.GetById(_objectGuid);
            if (output == null)
                return true;

            var state = OutputState.Unknown;
            if (_parameters[0] is OutputState)
                state = (OutputState)_parameters[0];

            var eventSources = new List<Guid>
                {
                    output.IdOutput
                };

            if (output.DCU != null)
            {
                eventSources.Add(output.DCU.IdDCU);
                if (output.DCU.CCU != null)
                    eventSources.Add(output.DCU.CCU.IdCCU);
            }
            else
                if (output.CCU != null)
                    eventSources.Add(output.CCU.IdCCU);

            var description =
                string.Format(
                    "Output {0} changed its state to {1}",
                    output,
                    state);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_OUTPUT_STATE_CHANGED,
                    _dateTime,
                    _thisAssemblyName,
                    eventSources,
                    description,
                    out _outEventlog,
                    out _outEventSources);
        }

        private bool HandleFunctionKeyPressed()
        {
            if (_parameters == null || _parameters.Length <= 1)
                return true;

            var state = _parameters[0] as State?;
            if (state == null)
                return true;

            var idOutput = _parameters[1] as Guid?;
            if (idOutput == null)
                return true;

            var cardReader = CardReaders.Singleton.GetById(_objectGuid);

            if (cardReader == null)
                return true;

            var eventSources =
                new LinkedList<Guid>(
                    CCUEventsManager.GetEventSourcesFromCardReader(cardReader));
            
            var output = Outputs.Singleton.GetById(idOutput.Value);

            if (output == null)
                return true;

            eventSources.AddLast(output.IdOutput);

            if (_parameters.Length > 2)
            {
                Card card = Cards.Singleton.GetById(_parameters[2]);

                if (card != null)
                {
                    eventSources.AddLast(card.IdCard);
                    eventSources.AddLast(card.Person.IdPerson);
                }
            }

            var description = string.Format(
                "The Function key changed the output {0} state to {1}",
                output,
                state.Value);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_FUNCTIONKEY_PRESSED,
                    _dateTime,
                    _thisAssemblyName,
                    eventSources,
                    description,
                    out _outEventlog,
                    out _outEventSources);
        }

        private bool HandleInputStateChanged()
        {
            if (_parameters == null)
                return true;

            var input = Inputs.Singleton.GetById(_objectGuid);
            if (input == null)
                return true;

            var state = InputState.Unknown;
            if (_parameters[0] is InputState)
                state = (InputState)_parameters[0];

            var eventSources =
                new List<Guid>
                {
                    input.IdInput
                };

            if (input.DCU != null)
            {
                eventSources.Add(input.DCU.IdDCU);
                if (input.DCU.CCU != null)
                    eventSources.Add(input.DCU.CCU.IdCCU);
            }
            else
                if (input.CCU != null)
                    eventSources.Add(input.CCU.IdCCU);

            if (_parameters.Length > 1)
            {
                if (_parameters.Length > 1)
                    eventSources.AddRange(
                        _parameters
                            .Skip(1)
                            .OfType<Guid>());
            }

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_INPUT_STATE_CHANGED,
                    _dateTime,
                    _thisAssemblyName,
                    eventSources,
                    string.Format(
                        "Input {0} changed its state to {1}",
                        input,
                        state),
                    out _outEventlog,
                    out _outEventSources);
        }

        private bool HandleCCUMemoryLoadStateChanged()
        {
            if (_parameters == null ||
                _parameters.Length != 1 ||
                !(_parameters[0] is State))
            {
                return true;
            }

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
            if (ccu == null)
                return true;

            var description = string.Empty;
            var memoryLoad = string.Empty;
            switch ((State) _parameters[0])
            {
                case State.Alarm:
                    description = string.Format("Memory load the CCU is high - above {0}%", NCASConstants.CCU_MEMORY_LOAD_TRESHOLD);
                    memoryLoad = "High";
                    break;
                case State.Normal:
                    description = string.Format("Memory load the CCU is low - below {0}%", NCASConstants.CCU_MEMORY_LOAD_TRESHOLD);
                    memoryLoad = "Low";
                    break;
            }

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_CCU_MEMORY_LOAD,
                    _dateTime,
                    _thisAssemblyName,
                    new[] {ccu.IdCCU},
                    description,
                    out _outEventlog,
                    out _outEventSources,
                    out _outEventlogParameters,
                    EventlogParameter.TYPE_CCU_MEMORY_LOAD,
                    memoryLoad);
        }

        private bool HandleCCUFilesystemProblem()
        {
            if (_parameters == null ||
                _parameters.Length != 3 ||
                !(_parameters[0] is State) ||
                !(_parameters[1] is string) ||
                !(_parameters[2] is string))
            {
                return true;
            }

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
            if (ccu == null)
                return true;

            var description = string.Empty;
            switch ((State)_parameters[0])
            {
                case State.Alarm:
                    description = "CCU filesystem problem occured, ";
                    break;
                case State.Normal:
                    description = "CCU filesystem problem expired, ";
                    break;
            }

            description += string.Format("file name: {0}, file operation: {1}", _parameters[1], _parameters[2]);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_CCU_FILESYSTEM_PROBLEM,
                    _dateTime,
                    _thisAssemblyName,
                    new[] { ccu.IdCCU },
                    description,
                    out _outEventlog,
                    out _outEventSources,
                    out _outEventlogParameters,
                    EventlogParameter.TYPE_FILE_NAME,
                    (string)_parameters[1],
                    EventlogParameter.TYPE_FILE_OPERATION,
                    (string)_parameters[2]);
        }

        private bool HandleCCUSdCardNotFound()
        {
            if (_parameters == null ||
                _parameters.Length != 1 ||
                !(_parameters[0] is State) ||
                (State)_parameters[0] != State.Alarm)
            {
                return true;
            }

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(_ccuIpAddress);
            if (ccu == null)
                return true;

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_CCU_SD_CARD_NOT_FOUND,
                    _dateTime,
                    _thisAssemblyName,
                    new[] { ccu.IdCCU },
                    "CCU SD card not found",
                    out _outEventlog,
                    out _outEventSources,
                    out _outEventlogParameters);
        }

        private static bool HandleAlarmAreaBoughtTimeChanged(EventLogBuilder builder)
        {
            /* This checking is in EventProcessor
            if (builder._parameters == null ||
                    builder._parameters.Length != 5
                    || !(builder._parameters[0] is Guid)    //Guid card reader
                    || !(builder._parameters[1] is Guid)    //Guid card or person
                    || !(builder._parameters[2] is Guid)    //Guid login
                    || !(builder._parameters[3] is int)     //int used time
                    || !(builder._parameters[4] is int))    //int remaining time
                return true;*/

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(builder._ccuIpAddress);
            if (ccu == null)
                return true;

            var alarmArea = AlarmAreas.Singleton.GetById(builder._objectGuid);
            if (alarmArea == null)
                return true;

            Func<int, string> timeConvert = (time) =>
            {
                if (time == int.MinValue)
                    return "UNLIMITED";

                if (time == int.MaxValue)
                    return "UNKNOW";

                int hour = time / 3600;
                int minute = time % 3600;
                int second = minute % 60;
                minute /= 60;

                return String.Format(
                    "{0:00}:{1:00}:{2:00}",
                    hour,
                    minute,
                    second);
            };

            string description;
            ICollection<Guid> eventSources;
            if (((Guid)builder._parameters[0]) != Guid.Empty)
            {
                //This event is from card reader
                var card = Cards.Singleton.GetById((Guid)builder._parameters[1]);
                var cardReader = CardReaders.Singleton.GetById(builder._parameters[0]);
                
                string personName = "UNKNOW";

                if(cardReader == null)
                {
                    // Card reader is unknow so add only alarm area and card to sources
                    eventSources = new LinkedList<Guid>();
                    eventSources.Add(alarmArea.IdAlarmArea);
                    if(card != null)
                    {
                        eventSources.Add(card.IdCard);
                        if(card.Person != null)
                        {
                            eventSources.Add((Guid)card.Person.GetId());
                            personName = card.Person.ToString();
                        }
                    }
                }
                else
                {
                    // If card reader is not null create sources
                    eventSources = new LinkedList<Guid>(
                    GetEventSourcesFromCardReader(
                        cardReader,
                        card,
                        alarmArea,
                        out personName));
                }

                var boughtTime = timeConvert((int)builder._parameters[3]);
                var remainingTime = timeConvert((int)builder._parameters[4]);

                if (card == null)
                {
                    description = string.Format(
                        "Alarm area was unset by GIN with time buying from card reader: {0}, alarm area: {1}, time bought: {2}, remaining time: {3}",
                        cardReader != null
                            ? cardReader.Name
                            : "UNKNOW",
                        alarmArea,
                        boughtTime,
                        remainingTime
                    );
                }
                else
                {
                    description = string.Format(
                        "Alarm area was unset by time buying from card reader: {0}, card: {1}, person: {2}, alarm area: {3}, time bought: {4}, remaining time: {5}",
                        cardReader != null
                            ? cardReader.Name
                            : "UNKNOW",
                        card.GetFullCardNumber(),
                        personName,
                        alarmArea,
                        boughtTime,
                        remainingTime
                    );
                }
                
            }
            else
            {
                //This event is from client
                var idLogin = (Guid)builder._parameters[2];
                
                if(idLogin == Guid.Empty)
                    return true;

                var login = Logins.Singleton.GetById(idLogin);
                if (login == null)
                    return true;

                eventSources = new LinkedList<Guid>();
                eventSources.Add(ccu.IdCCU);
                eventSources.Add(alarmArea.IdAlarmArea);
                eventSources.Add(idLogin);

                var timeToBuy = timeConvert((int)builder._parameters[3]);
                var remainingTime = timeConvert((int)builder._parameters[4]);

                if (login.Person != null)
                {
                    //this login has person
                    eventSources.Add(login.Person.IdPerson);
                    description = string.Format(
                        "Alarm area was unset by time buying from client by person {0}, alarm area: {1}, time bought: {2}, remaining time: {3}",
                        login.Person.WholeName,
                        alarmArea,
                        timeToBuy,
                        remainingTime
                    );
                }
                else
                {
                    //this login does not have person
                    description = string.Format(
                        "Alarm area was unset by time buying from client by login {0}, alarm area: {1}, time bought: {2}, remaining time: {3}",
                        login,
                        alarmArea,
                        timeToBuy,
                        remainingTime
                    );
                }
            }

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_ALARM_AREA_BOUGHT_TIME_CHANGED,
                    builder._dateTime,
                    builder._thisAssemblyName,
                    eventSources,
                    description,
                    out builder._outEventlog,
                    out builder._outEventSources,
                    out builder._outEventlogParameters);
        }

        private static bool HandleAlarmAreaBoughtTimeExpired(EventLogBuilder builder)
        {
            /* This checking is in EventProcessor
            if (builder._parameters == null ||
                    builder._parameters.Length != 2
                    || !(builder._parameters[0] is int)     //int used time
                    || !(builder._parameters[1] is int))    //int totalBoughtTime
                return true;*/

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(builder._ccuIpAddress);
            if (ccu == null)
                return true;

            var alarmArea = AlarmAreas.Singleton.GetById(builder._objectGuid);
            if (alarmArea == null)
                return true;

            ICollection<Guid> eventSources = new LinkedList<Guid>();

            eventSources.Add(ccu.IdCCU);
            eventSources.Add(alarmArea.IdAlarmArea);

            Func<int, string> timeConvert = (time) =>
            {
                if (time == int.MinValue)
                    return "UNLIMITED";

                if (time == int.MaxValue)
                    return "UNKNOW";

                int hour = time / 3600;
                int minute = time % 3600;
                int second = minute % 60;
                minute /= 60;

                return String.Format(
                    "{0:00}:{1:00}:{2:00}",
                    hour,
                    minute,
                    second);
            };

            var boughtTime = timeConvert((int)builder._parameters[0]);
            var totalBoughtTime = timeConvert((int)builder._parameters[1]);

            var description = string.Format("Bought time for alarm area \"{0}\" expired. Bought time: {1}. Total bought time: {2}",
                alarmArea,
                boughtTime.ToString(),
                totalBoughtTime.ToString());
            
            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_ALARM_AREA_BOUGHT_TIME_EXPIRED,
                    builder._dateTime,
                    builder._thisAssemblyName,
                    eventSources,
                    description,
                    out builder._outEventlog,
                    out builder._outEventSources,
                    out builder._outEventlogParameters);
        }

        private static bool HandleAlarmAreaTimeBuyingFailed(EventLogBuilder builder)
        {
            /* This checking is in EventProcessor
            if (builder._parameters == null ||
                    builder._parameters.Length != 6
                    || !(builder._parameters[0] is byte)    //byte reason
                    || !(builder._parameters[1] is Guid)    //Guid guidLogin
                    || !(builder._parameters[2] is Guid)    //Guid guidPerson
                    || !(builder._parameters[3] is int))    //int timeToBuy
                    || !(builder._parameters[4] is int))    //int timeToBuy
                    || !(builder._parameters[5] is Guid[])) //Guid[] sources
                return true;*/

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(builder._ccuIpAddress);
            if (ccu == null)
                return true;

            var alarmArea = AlarmAreas.Singleton.GetById(builder._objectGuid);
            if (alarmArea == null)
                return true;

            var person = "UNKNOW";
            var login = "UNKNOW";

            ICollection<Guid> eventSources = new HashSet<Guid>();

            eventSources.Add(ccu.IdCCU);
            
            var idAlarmArea = builder._objectGuid;
            eventSources.Add(idAlarmArea);

            // Get person name from login
            var idLogin = (Guid)builder._parameters[1];
            if (idLogin != Guid.Empty)
            {
                if (!eventSources.Contains(idLogin))
                    eventSources.Add(idLogin);

                var tempLogin = Logins.Singleton.GetById(idLogin);
                if (tempLogin != null)
                {
                    login = tempLogin.ToString();

                    if (tempLogin.Person != null)
                    {
                        if (!eventSources.Contains(tempLogin.Person.IdPerson))
                            eventSources.Add(tempLogin.Person.IdPerson);

                        person = tempLogin.Person.ToString();
                    }
                }
            }

            // Get person name from card
            var idPerson = (Guid)builder._parameters[2];
            if (idPerson != Guid.Empty)
            {
                if (!eventSources.Contains(idPerson))
                    eventSources.Add(idPerson);

                var tempPerson = Persons.Singleton.GetById(idPerson);

                if (tempPerson != null)
                    person = tempPerson.ToString();
            }

            Func<int, string> timeConvert = (time) =>
            {
                if (time == int.MinValue)
                    return "UNLIMITED";

                if (time == int.MaxValue)
                    return "UNKNOW";

                int hour = time / 3600;
                int minute = time % 3600;
                int second = minute % 60;
                minute /= 60;

                return String.Format(
                    "{0:00}:{1:00}:{2:00}",
                    hour,
                    minute,
                    second);
            };

            var aaaResult = (AlarmAreaActionResult)builder._parameters[0];
            var timeToBuy = TimeSpan.FromSeconds((int)builder._parameters[3]);

            string remainingTime;
            string remainingOrMissing;
            if ((int)builder._parameters[4] >= 0)
            {
                remainingTime = timeConvert((int)builder._parameters[4]);
                remainingOrMissing = "remaining";
            }
            else
            {
                remainingTime = timeConvert((int)builder._parameters[4] * -1);
                remainingOrMissing = "missing";
            }

            var description = string.Format(
                "Alarm area time buying failed because of \"{0}\". Alarm area: {1}. Invoked by login: {2}, person: {3}, time to buy: {4}, {5} time: {6}",
                aaaResult,
                alarmArea,
                login,
                person,
                timeToBuy,
                remainingOrMissing,
                remainingTime
                );

            // Add sources from ccu
            var sourcesFromCCU = (Guid[])builder._parameters[5];
            foreach (var source in sourcesFromCCU)
                if (!eventSources.Contains(source))
                    eventSources.Add(source);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_ALARM_AREA_TIME_BUYING_FAILED,
                    builder._dateTime,
                    builder._thisAssemblyName,
                    eventSources,
                    description,
                    out builder._outEventlog,
                    out builder._outEventSources,
                    out builder._outEventlogParameters);
        }

        private static bool HandleAntiPassBackZoneCardEntered(EventLogBuilder builder)
        {
            var guidAntiPassBackZone = builder._objectGuid;

            var guidCard = (Guid)builder._parameters[0];
            var guidEntryCardReader = (Guid)builder._parameters[1];
            var accessInterrupted = (bool)builder._parameters[2];

            ICollection<Guid> eventSources =
                new LinkedList<Guid>();

            var card = 
                Cards.Singleton.GetById(guidCard);

            Person person = null;

            if (card != null)
            {
                eventSources.Add(card.IdCard);

                person = card.Person;

                if (person != null)
                    eventSources.Add(person.IdPerson);
            }

            var antiPassBackZone = 
                AntiPassBackZones.Singleton.GetById(guidAntiPassBackZone);

            if (antiPassBackZone != null)
                eventSources.Add(antiPassBackZone.IdAntiPassBackZone);

            var entryCardReader =
                CardReaders.Singleton.GetById(guidEntryCardReader);

            if (entryCardReader != null)
            {
                eventSources.Add(entryCardReader.IdCardReader);

                var dcu = entryCardReader.DCU;

                CCU ccu;

                if (dcu != null)
                {
                    eventSources.Add(dcu.IdDCU);

                    ccu = dcu.CCU;
                }
                else
                    ccu = entryCardReader.CCU;

                if (ccu != null)
                    eventSources.Add(ccu.IdCCU);
            }

            var stringBuilder =
                new StringBuilder("Card ");

            if (card != null)
            {
                stringBuilder.Append(card.Name);
                stringBuilder.Append(' ');

                if (person != null)
                    stringBuilder.AppendFormat(
                        "(person name \"{0}\") ",
                        person.WholeName);
            }

            stringBuilder.Append("entered anti-passback zone ");

            if (antiPassBackZone != null)
                stringBuilder.AppendFormat(
                    "\"{0}\" ",
                    antiPassBackZone.Name);

            if (entryCardReader != null)
                stringBuilder.AppendFormat(
                    "via {0} on card reader {1}",
                    accessInterrupted
                        ? "access interrupted"
                        : "normal access",
                    entryCardReader.Name);
            else
                stringBuilder.Append("without card reader access");

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_ANTI_PASS_BACK_ZONE_CARD_ENTERED,
                    builder._dateTime,
                    builder._thisAssemblyName,
                    eventSources,
                    stringBuilder.ToString(),
                    out builder._outEventlog,
                    out builder._outEventSources,
                    out builder._outEventlogParameters);
        }

        private static bool HandleAntiPassBackZoneCardExited(EventLogBuilder builder)
        {
            var guidAntiPassBackZone = builder._objectGuid;

            var guidCard = (Guid)builder._parameters[0];
            var guidEntryCardReader = (Guid)builder._parameters[1];
            var accessInterrupted = (bool)builder._parameters[2];
            var entryDateTime = (DateTime)builder._parameters[3];
            var guidExitCardReader = (Guid)builder._parameters[4];

            ICollection<Guid> eventSources =
                new LinkedList<Guid>();

            var card =
                Cards.Singleton.GetById(guidCard);

            Person person = null;

            if (card != null)
            {
                eventSources.Add(card.IdCard);

                person = card.Person;

                if (person != null)
                    eventSources.Add(person.IdPerson);
            }

            var antiPassBackZone =
                AntiPassBackZones.Singleton.GetById(guidAntiPassBackZone);

            if (antiPassBackZone != null)
                eventSources.Add(antiPassBackZone.IdAntiPassBackZone);

            var exitCardReader =
                CardReaders.Singleton.GetById(guidExitCardReader);

            if (exitCardReader != null)
            {
                eventSources.Add(exitCardReader.IdCardReader);

                var dcu = exitCardReader.DCU;

                CCU ccu;

                if (dcu != null)
                {
                    eventSources.Add(dcu.IdDCU);

                    ccu = dcu.CCU;
                }
                else
                    ccu = exitCardReader.CCU;

                if (ccu != null)
                    eventSources.Add(ccu.IdCCU);
            }

            var stringBuilder =
                new StringBuilder("Card ");

            if (card != null)
            {
                stringBuilder.Append(card.Number);
                stringBuilder.Append(' ');

                if (person != null)
                    stringBuilder.AppendFormat(
                        "(person name \"{0}\") ",
                        person.WholeName);
            }

            stringBuilder.Append("exited from anti-passback zone ");

            if (antiPassBackZone != null)
                stringBuilder.AppendFormat(
                    "\"{0}\" ",
                    antiPassBackZone.Name);

            if (exitCardReader != null)
                stringBuilder.AppendFormat(
                    "via normal access on card reader {0} ",
                    exitCardReader.Name);

            stringBuilder.AppendFormat(
                "[entered on {0} ",
                entryDateTime);

            var entryCardReader =
                CardReaders.Singleton.GetById(guidEntryCardReader);

            if (entryCardReader != null)
                stringBuilder.AppendFormat(
                    "via {0} on card reader {1}",
                    accessInterrupted
                        ? "access interrupted"
                        : "normal access",
                    entryCardReader.Name);
            else
                stringBuilder.Append("without card reader access");

            stringBuilder.Append(']');

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_ANTI_PASS_BACK_ZONE_CARD_EXITED,
                    builder._dateTime,
                    builder._thisAssemblyName,
                    eventSources,
                    stringBuilder.ToString(),
                    out builder._outEventlog,
                    out builder._outEventSources,
                    out builder._outEventlogParameters);
        }

        private static bool HandleAntiPassBackZoneCardTimedOut(EventLogBuilder builder)
        {
            var guidAntiPassBackZone = builder._objectGuid;

            var guidCard = (Guid)builder._parameters[0];
            var guidEntryCardReader = (Guid)builder._parameters[1];
            var accessInterrupted = (bool)builder._parameters[2];
            var entryDateTime = (DateTime)builder._parameters[3];

            ICollection<Guid> eventSources =
                new LinkedList<Guid>();

            var card =
                Cards.Singleton.GetById(guidCard);

            Person person = null;

            if (card != null)
            {
                eventSources.Add(card.IdCard);

                person = card.Person;

                if (person != null)
                    eventSources.Add(person.IdPerson);
            }

            var antiPassBackZone =
                AntiPassBackZones.Singleton.GetById(guidAntiPassBackZone);

            if (antiPassBackZone != null)
            {
                eventSources.Add(antiPassBackZone.IdAntiPassBackZone);
                var ccu = antiPassBackZone.GetParentCCU();

                if (ccu != null)
                    eventSources.Add(ccu.IdCCU);
            }

            var stringBuilder =
                new StringBuilder("Card ");

            if (card != null)
            {
                stringBuilder.Append(card.Number);
                stringBuilder.Append(' ');

                if (person != null)
                    stringBuilder.AppendFormat(
                        "(person name \"{0}\") ",
                        person.WholeName);
            }

            stringBuilder.Append("exited from anti-passback zone ");

            if (antiPassBackZone != null)
                stringBuilder.AppendFormat(
                    "\"{0}\" ",
                    antiPassBackZone.Name);

            stringBuilder.AppendFormat(
                "after timeout [entered on {0} ",
                entryDateTime);

            var entryCardReader =
                CardReaders.Singleton.GetById(guidEntryCardReader);

            if (entryCardReader != null)
                stringBuilder.AppendFormat(
                    "via {0} on card reader {1}",
                    accessInterrupted
                        ? "access interrupted"
                        : "normal access",
                    entryCardReader.Name);
            else
                stringBuilder.Append("without card reader access");

            stringBuilder.Append(']');

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_ANTI_PASS_BACK_ZONE_CARD_TIMED_OUT,
                    builder._dateTime,
                    builder._thisAssemblyName,
                    eventSources,
                    stringBuilder.ToString(),
                    out builder._outEventlog,
                    out builder._outEventSources,
                    out builder._outEventlogParameters);
        }

        private static bool HandleSetAlarmAreaFromClient(EventLogBuilder builder)
        {
            var guidAlarmArea = builder._objectGuid;
            var guidLogin = (Guid)builder._parameters[0];
            var noPrewarning = (bool) builder._parameters[1];

            var alarmArea = AlarmAreas.Singleton.GetById(guidAlarmArea);
            if (alarmArea == null)
                return true;

            var login = Logins.Singleton.GetById(guidLogin);
            if (login == null)
                return true;

            ICollection<Guid> eventSources =
                new LinkedList<Guid>();

            eventSources.Add(alarmArea.IdAlarmArea);

            eventSources.Add(login.IdLogin);

            if (login.Person != null)
                eventSources.Add(login.Person.IdPerson);

            string info = noPrewarning
                ? " with no prewarning"
                : string.Empty;

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_SET_ALARM_AREA_FROM_CLIENT,
                    builder._dateTime,
                    builder._thisAssemblyName,
                    eventSources,
                    login.Person != null
                        ? string.Format(
                            "Set alarm area from client, login: {0}, person: {1}, alarm area: {2}{3}",
                            login,
                            login.Person,
                            alarmArea,
                            info)
                        : string.Format(
                            "Set alarm area from client, login: {0}, alarm area: {1}{2}",
                            login,
                            alarmArea,
                            info),
                    out builder._outEventlog,
                    out builder._outEventSources,
                    out builder._outEventlogParameters);
        }

        private static bool HandleUnsetAlarmAreaFromClient(EventLogBuilder builder)
        {
            var guidAlarmArea = builder._objectGuid;
            var guidLogin = (Guid)builder._parameters[0];
            var timeBuying = (int) builder._parameters[1];

            var alarmArea = AlarmAreas.Singleton.GetById(guidAlarmArea);
            if (alarmArea == null)
                return true;

            var login = Logins.Singleton.GetById(guidLogin);
            if (login == null)
                return true;

            ICollection<Guid> eventSources =
                new LinkedList<Guid>();

            eventSources.Add(alarmArea.IdAlarmArea);

            eventSources.Add(login.IdLogin);

            if (login.Person != null)
                eventSources.Add(login.Person.IdPerson);


            string info = timeBuying > 0
                ? String.Format(
                    " with time buying {0:00}:{1:00}:{2:00}",
                    timeBuying/3600,
                    (timeBuying%3600)/60,
                    timeBuying%60)
                : string.Empty;

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_UNSET_ALARM_AREA_FROM_CLIENT,
                    builder._dateTime,
                    builder._thisAssemblyName,
                    eventSources,
                    login.Person != null
                        ? string.Format(
                            "Unset alarm area from client, login: {0}, person: {1}, alarm area: {2}{3}",
                            login,
                            login.Person,
                            alarmArea,
                            info)
                        : string.Format(
                            "Unset alarm area from client, login: {0}, alarm area: {1}{2}",
                            login,
                            alarmArea,
                            info),
                    out builder._outEventlog,
                    out builder._outEventSources,
                    out builder._outEventlogParameters);
        }

        private static string[] CreateDsmEventsParameters(
            object[] parameters,
            IEnumerable<Guid> eventSources,
            out IEnumerable<Guid> resultEventSources)
        {
            var newEventSources =
                new LinkedList<Guid>(eventSources);

            var result =
                CreateDsmEventsParametersInternal(
                    parameters,
                    newEventSources);

            resultEventSources = newEventSources;

            return
                result != null
                    ? result.ToArray()
                    : null;
        }

        private static IEnumerable<string> CreateDsmEventsParametersInternal(
            object[] parameters,
            ICollection<Guid> eventSources)
        {
            if (parameters.Length == 1 && (string)parameters[0] == string.Empty)
                return null;

            ICollection<string> result = new LinkedList<string>();

            if (parameters.Length <= 1)
            {
                if (parameters.Length <= 0)
                    return result;

                result.Add("Reason");
                result.Add((string)parameters[0]);

                return result;
            }

            if (parameters[0] is Guid &&
                (Guid)parameters[0] != Guid.Empty)
            {
                result.Add("CardReaderGuid");
                result.Add(parameters[0].ToString());

                var cardReader = CardReaders.Singleton.GetById((Guid)parameters[0]);
                if (cardReader != null)
                    eventSources.Add(cardReader.IdCardReader);
            }

            if (parameters[1] is Guid &&
                (Guid)parameters[1] != Guid.Empty)
            {
                var card = Cards.Singleton.GetById((Guid)parameters[1]);
                if (card != null)
                {
                    result.Add("Card number");
                    result.Add(card.GetFullCardNumber());
                    eventSources.Add(card.IdCard);

                    if (card.Person != null)
                    {
                        result.Add("User name");
                        result.Add(
                            card.Person.FirstName + StringConstants.SPACE + card.Person.Surname);
                        eventSources.Add(card.Person.IdPerson);
                    }
                }
            }
            else
                if (parameters.Length == 4 &&
                    parameters[1] is Guid &&
                    (Guid)parameters[1] == Guid.Empty &&
                    parameters[2] is string &&
                    (string)parameters[2] != string.Empty)
                {
                    result.Add("Card number");
                    result.Add((string)parameters[2]);

                    var card = Cards.Singleton.GetCardFromFullCardNumber((string)parameters[2]);
                    if (card != null)
                        eventSources.Add(card.IdCard);
                }

            if (parameters.Length == 2)
                return result;

            if (parameters.Length == 3 &&
                parameters[2] is string)
            {
                result.Add("Reason");
                result.Add((string)parameters[2]);

                return result;
            }

            if (parameters[2] is Guid &&
                (Guid)parameters[2] != Guid.Empty)
            {
                result.Add("PushButtonGuid");
                result.Add(parameters[2].ToString());

                var pushButton = Inputs.Singleton.GetById((Guid)parameters[2]);
                if (pushButton != null)
                    eventSources.Add(pushButton.IdInput);
            }

            if (parameters.Length != 4)
                return result;

            result.Add("Reason");
            result.Add((string)parameters[3]);

            return result;
        }
    }
}
